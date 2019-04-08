using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WaveSettingsPreciseData))]
public class PreciseDataEditor : Editor
{
    private int currentIdx;

    public override void OnInspectorGUI()
    {
        var me = target as WaveSettingsPreciseData;

        currentIdx = EditorGUILayout.IntSlider(currentIdx, 0, me.datas.Count - 1);

        base.OnInspectorGUI();
        //var property = serializedObject.FindProperty("datas");
        //EditorGUILayout.PropertyField(property);

        foreach (var d in me.datas)
        {
            int i = 0;
            foreach (var w in d.waves)
            {
                EditorGUILayout.LabelField("element " + i++ + "-- direction is " + w.Direction2D);
            }
        }

        //serializedObject.Update();
        if (!Application.isPlaying) return;
        if (GUILayout.Button("Play"))
        {
            var c = GameObject.FindObjectOfType<ObjectSpawner>();
            if (c != null)
            {
                if (target as WaveSettingsPreciseData != null)
                    c.MoveNextWave<WaveSettingsPrecise>(target as WaveSettingsData);
                else
                    c.MoveNextWave<WaveSettingsRapid>(target as WaveSettingsData);
            }
        }
    }
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        foreach (var data in (target as WaveSettingsPreciseData).datas)
            foreach (var w in data.waves)
            {
                if (w.rotation.w == 0)
                    w.rotation.w = 1;
            }
    }


    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    private void OnSceneGUI(SceneView v)
    {
        var me = target as WaveSettingsPreciseData;
        int i = 0;
        foreach (var data in me.datas)
        {
            if (i++ != currentIdx) continue;

            foreach (var w in data.waves)
            {
                var c = Handles.color;
                Handles.color = Color.green;
                w.spawnPoint = Handles.FreeMoveHandle(w.spawnPoint, Quaternion.identity, 0.6f, Vector3.one, Handles.DotHandleCap);
                Handles.color = Color.blue;

                w.rotation = Handles.RotationHandle(w.rotation, w.spawnPoint);
                //var euler = rot.eulerAngles;
                //euler.x = 0;
                //euler.y = 0;

                Handles.color = Color.white;
                var id = GUIUtility.GetControlID(FocusType.Passive);
                Handles.ArrowHandleCap(id, w.spawnPoint, Quaternion.LookRotation(w.Direction2D), 2f, EventType.Repaint);
                Handles.color = c;
            }
        }
    }
}

#endif

[CreateAssetMenu(menuName = "Gun Data/WaveSettingsPrecise")]
public class WaveSettingsPreciseData : WaveSettingsData
{
    [System.Serializable]
    public class DataFormatter
    {
        public Vector2 spawnPoint;
        public Quaternion rotation;
        public float force;

        public Vector2 Direction2D
        {
            get
            {
                return rotation * Vector2.left;
            }
        }
    }
    [System.Serializable]
    public class Data
    {
        public List<DataFormatter> waves;
    }

    public float delayBetween = 1f;
    //public List<List<DataFormatter>> datas;
    public List<Data> datas;
}
