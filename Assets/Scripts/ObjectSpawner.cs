using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectSpawner : SingletonBehaviour<ObjectSpawner>
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectSpawner))]
    class ObjectSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var me = target as ObjectSpawner;

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Start Wave"))
                    me.StartWaves();
                if (GUILayout.Button("Stop Wave"))
                    me.StopWaves();
            }
            else
            {
                if (GUILayout.Button("Setup"))
                {
                    List<WaveSettingsData> datas = new List<WaveSettingsData>();
                    string[] assetsGuid = AssetDatabase.FindAssets("", new string[] { "Assets/Data" });
                    foreach (var guid in assetsGuid)
                        datas.Add(AssetDatabase.LoadAssetAtPath<WaveSettingsData>(AssetDatabase.GUIDToAssetPath(guid)));

                    me.dataSource = datas;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        //draw range of spawn
        var c = Gizmos.color;
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        Gizmos.DrawCube(offsetPosition, offsetRange);
        Gizmos.color = c;
    }
    private Vector3 offsetPosition;
    private Vector2 offsetRange;
    public void DrawOffset(WaveStartPosition sp, Vector2 offMin, Vector2 offMax)
    {
        this.offsetRange = offMax - offMin;

        this.offsetPosition = GetStartPosition(sp) + (Vector3)Vector2.Lerp(offMin, offMax, 0.5f);

        if (offsetRange.x <= 0.05f)
            offsetRange.x = 0.5f;
        if (offsetRange.y <= 0.05f)
            offsetRange.y = 0.5f;

        Debug.Log("draw offset called => " + offsetPosition + "   " + offsetRange);
    }
#endif


    List<SmashableObject> activeObjects;
    public List<SmashableObject> ActiveObjects
    {
        get
        {
            if (activeObjects == null)
                activeObjects = new List<SmashableObject>();
            return activeObjects;
        }
    }

    [Header("Spawner")]
    [Header("NB. Soy Must be the first prefab")]
    public List<SmashableObject> prefabs;
    public Transform rightSpawnPos;
    public Transform bottomSpawnPos;

    [Header("Params")]
    public float soyPercentages = 10f;
    public int minWavesBetweenSoy = 10;
    public WaveLevel startLevel, maxLevel;
    public int increaseLevelAfterWaves;

    private int wavesFromLastSoy;
    private WaveLevel currentLevel;
    private int wavesFromLastLevelIncrease;

    [Header("Instances")]
    public bool autoDestroyInstances = true;
    public float autoDestoryTime = 10f;

    [SerializeField]
    private List<WaveSettingsData> dataSource;
    private Dictionary<WaveLevel, Dictionary<System.Type, List<WaveSettingsData>>> dataMap;
    private Dictionary<WaveLevel, List<SmashableObject>> perLevelPrefabs;

    SmashableObject RandomPrefab
    {
        get
        {
            //could be soy?
            bool couldBeSoy = wavesFromLastSoy >= minWavesBetweenSoy;
            bool willBeSoy = couldBeSoy && Random.Range(0f, 100f) <= soyPercentages;

            if (willBeSoy)
            {
                wavesFromLastSoy = 0;
                return GetSoyPrefab();
            }

            return GetRandomPrefabForLevel(this.currentLevel);
        }
    }
    SmashableObject GetSoyPrefab()
    {
        return prefabs[0];
    }
    SmashableObject GetRandomPrefabForLevel(WaveLevel level)
    {
        int idx = Random.Range(0, perLevelPrefabs[level].Count);
        return perLevelPrefabs[level][idx];
    }

    private WaveSettings currentWave;

    private void Awake()
    {
        Instance = this;

        dataMap = new Dictionary<WaveLevel, Dictionary<System.Type, List<WaveSettingsData>>>();
        foreach (var d in dataSource)
        {
            if (!dataMap.ContainsKey(d.level))
                dataMap.Add(d.level, new Dictionary<System.Type, List<WaveSettingsData>>());
            if (!dataMap[d.level].ContainsKey(d.GetType()))
                dataMap[d.level].Add(d.GetType(), new List<WaveSettingsData>());

            dataMap[d.level][d.GetType()].Add(d);
        }

        //build per level prefabs map, it not contains soy
        perLevelPrefabs = new Dictionary<WaveLevel, List<SmashableObject>>();
        foreach (var prefab in prefabs)
        {
            if (prefab.IsSoy) continue;

            foreach (var level in prefab.availableLevels)
            {
                if (!perLevelPrefabs.ContainsKey(level))
                    perLevelPrefabs.Add(level, new List<SmashableObject>());

                perLevelPrefabs[level].Add(prefab);
            }
        }
    }
    private void Start()
    {
#if UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TEST")
            return;
#endif
        GameManager.Instance.onMatchLostBefore += () => StopWaves();
        GameManager.Instance.onMatchStart += StartWaves;
    }

    Vector3 GetStartPosition(WaveStartPosition wsp)
    {
        return wsp == WaveStartPosition.Bottom ? this.bottomSpawnPos.position : this.rightSpawnPos.position;
    }

    public void StartWaves()
    {
        currentLevel = startLevel;
        wavesFromLastLevelIncrease = 0;

#if M_DEBUG
#if UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TEST")
#endif
            //UIDebugPage.Instance.SetLevel("level", currentLevel.ToString());
#endif

        MoveNextWave();
    }
    public void StopWaves(bool isGameOver = true, bool isNewRecord = false)
    {
        StopCoroutine(currentWave.activeCoroutine);
        var objs = GameObject.FindObjectsOfType<SmashableObject>();
        for (int i = objs.Length - 1; i >= 0; i--)
        {
            objs[i].RemoveForEndMatchSmashed();
        }
    }

    bool DataTypeExistsAtLevel(WaveLevel level, System.Type t)
    {
        return dataMap[level].ContainsKey(t);
    }
    System.Type GetDataTypeFromSettings<T>() where T : WaveSettings
    {
        if (typeof(T) == typeof(WaveSettingsPrecise))
            return typeof(WaveSettingsPreciseData);

        return typeof(WaveSettingsRapidData);
    }
    public WaveSettingsData GetRandomWaveData<T>(WaveLevel level) where T : WaveSettings
    {
        System.Type t = GetDataTypeFromSettings<T>();
        if (!DataTypeExistsAtLevel(level, t))
        {
            //if (typeof(T) == typeof(WaveSettingsRapid))
            //    t = GetDataTypeFromSettings<WaveSettingsPrecise>();
            //else
            //    t = GetDataTypeFromSettings<WaveSettingsRapid>();
            throw new UnityException("Requested type " + t.Name + " is not present at level " + level);
        }

        int idx = Random.Range(0, dataMap[level][t].Count);
        return dataMap[level][t][idx];
    }

    public void MoveNextWave()
    {
        float idx = Random.Range(0f, 1f);
        if (idx > .5f)
            MoveNextWave<WaveSettingsRapid>();
        else
            MoveNextWave<WaveSettingsPrecise>();
    }
    void MoveNextWave<Wave>() where Wave : WaveSettings, new()
    {
        MoveNextWave<Wave>(GetRandomWaveData<Wave>(currentLevel), false);
    }
    public void MoveNextWave<Wave>(WaveSettingsData data, bool oneShot = true) where Wave : WaveSettings, new()
    {
        currentWave = WaveSettings.Get<Wave>(this, data);

        currentWave.oneShot = oneShot;
        currentWave.activeCoroutine = currentWave.Wave();
        StartCoroutine(currentWave.activeCoroutine);
    }

    public void IncreaseWaveCount()
    {
        //increase wave count for soy
        wavesFromLastSoy++;
        wavesFromLastLevelIncrease++;

        if (wavesFromLastLevelIncrease > increaseLevelAfterWaves)
        {
            wavesFromLastLevelIncrease = 0;
            if (currentLevel == maxLevel) return;   //can't increase level

            currentLevel++;
#if M_DEBUG
#if UNITY_EDITOR
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TEST")
                return;
#endif
            //UIDebugPage.Instance.SetLevel("level", "Level = " + currentLevel.ToString());
#endif
        }
    }

    /// <summary>
    /// You should increase wave count manually
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <param name="dir"></param>
    /// <param name="force"></param>
    public void SimulateOne(Vector2 spawnPos, Vector2 dir, float force)
    {
        var instance = Instantiate(RandomPrefab, spawnPos, Quaternion.identity);
        instance.Throw(dir * force);

        if (autoDestroyInstances)
            Destroy(instance.gameObject, autoDestoryTime);
    }
    public void SimulateOneForTutorial(WaveSettingsPreciseData data, SmashableObject instance)
    {
        var d = data.datas[0].waves[0];
        instance.transform.position = d.spawnPoint;
        instance.Throw(d.Direction2D * d.force);
    }
    public void SimulateOneWave(int n, List<Vector2> dirs, List<float> forces, List<Vector2> offsets, WaveStartPosition startPos)
    {
        var p = GetStartPosition(startPos);
        for (int i = 0; i < n; i++)
        {
            var instance = Instantiate(RandomPrefab, p + (Vector3)offsets[i], Quaternion.identity);
            instance.Throw(dirs[i] * forces[i]);

            if (autoDestroyInstances)
                Destroy(instance.gameObject, autoDestoryTime);
        }

        IncreaseWaveCount();
    }
}





