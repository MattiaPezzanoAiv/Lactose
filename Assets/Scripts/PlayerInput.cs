using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TrailData
{
    public GameObject trail;
    public string name;
    public string lockedLabel;
    public int minScore;
    public bool unlockedByAchiev;
    public string achievName;

    public bool IsScoreEnough()
    {
        int myBestScore = GameManager.Instance.GetBestScore();
        return myBestScore >= minScore;
    }
    public bool IsOkForAchiev()
    {
        if (!unlockedByAchiev) return true;

        return AchievmentsManager.Instance.IsAchievmentUnlocked(achievName);
    }
}

public class PlayerInput : SingletonBehaviour<PlayerInput>
{
    //private LineRenderer line;
    private new EdgeCollider2D collider;

    [SerializeField]
    private float colliderPopStepTime, linePopStepTime;
    [SerializeField]
    private int maxColliderPointsAllowed, maxLinePointsAllowed;
    [SerializeField]
    private Color startColor, endColor;
    [SerializeField]
    private float startW = 0.1f, endW = 0.3f;
    [SerializeField]
    private float colliderSize = 0.3f, collidersTreshold;

    [SerializeField]
    private List<TrailData> playerTrailsList;
    private GameObject currentTrail;

    private IEnumerator colliderCoroutine;
    //private IEnumerator lineCoroutine;

    public bool ColliderCoroutineActive { get { return colliderCoroutine != null; } }
    //public bool LineCoroutineActive { get { return lineCoroutine != null; } }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerInput))]
    class CustomEditorPlayerInput : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as PlayerInput;
            EditorGUILayout.LabelField("Collider Coroutine Active => " + me.ColliderCoroutineActive);
            //EditorGUILayout.LabelField("Line Coroutine Active => " + me.LineCoroutineActive);

            base.OnInspectorGUI();
        }
    }
#endif

    //int i = 0;
    private void Awake()
    {
        Instance = this;

        collider = GetComponent<EdgeCollider2D>();
        collider.edgeRadius = colliderSize;

        //line = GetComponent<LineRenderer>();
        //line.enabled = false;
        //line.useWorldSpace = true;

        //line.startColor = startColor;
        //line.endColor = endColor;
        //line.startWidth = startW;
        //line.endWidth = endW;
        //line.positionCount = 0;

        //line.numCornerVertices = 5;
    }
    private void Start()
    {
#if UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TEST")
            return;
#endif
        //GameManager.Instance.onMatchLost += (gameOver) => Disable();
        GameManager.Instance.onMatchStart += () => Enable();
        GameManager.Instance.onFirstNewBestScoreInMatch += (x) => Disable();
        GameManager.Instance.onFirstNewBestScoreInMatchEnd += () => Enable();

        //GameManager.Instance.onMatchPause += () =>
        //{
        //    Disable();
        //};
        GameManager.Instance.onMatchResume += () =>
        {
            Enable();
        };

        var idx = SettingsManager.Instance.GetSavedTrail();
        SetupTrail(idx, false);

        Enable();
    }


    #region PARTICLE_TRAIL
    public void SetupTrail(int idx, bool saveNewTrail)
    {
        if (idx >= playerTrailsList.Count)
        {
            idx = playerTrailsList.Count - 1;
        }
        if (currentTrail != null)
            Destroy(currentTrail);

        currentTrail = Instantiate(playerTrailsList[idx].trail, transform);
        currentTrail.transform.position = Vector3.zero;

        if (saveNewTrail)
            SettingsManager.Instance.SaveTrail(idx);
    }
    public List<TrailData> GetTrailData()
    {
        return playerTrailsList;
    }
    //public void ManuallyUnlockTrail(string name)
    //{
    //    foreach (var trail in playerTrailsList)
    //    {
    //        if(trail.name == name)
    //        {

    //            break;
    //        }
    //    }
    //}
    //public List<string> GetAvailableTrails(ref int current)
    //{
    //    current = SettingsManager.Instance.GetSavedTrail();
    //    return (from go in playerTrailsList select go.trail.name).ToList();
    //}
    //public List<bool> GetUnlockedTrails()
    //{
    //    var myBestScore = GameManager.Instance.GetBestScore();
    //    return (from trail in playerTrailsList select myBestScore >= trail.minScore).ToList();
    //}
    //public List<Color> GetAvailableTrailsColors()
    //{
    //    return (from go in playerTrailsList
    //            where go.trail.GetComponent<ParticleSystem>() != null
    //            select go.trail.GetComponent<ParticleSystem>().main.startColor.color).ToList();
    //}
    #endregion



    public void Disable()
    {
        //EmptyLine();
        EmptyCollider();
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
        StartAllCoroutines();
    }

    bool TouchAvailable()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0;
#endif
    }
    bool TouchMoving()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        Touch touch = Input.GetTouch(0);
        return touch.phase == TouchPhase.Moved;
#endif
    }
    bool TouchStarting()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        Touch touch = Input.GetTouch(0);
        return touch.phase == TouchPhase.Began;
#endif
    }
    bool TouchEnding()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
#else
        Touch touch = Input.GetTouch(0);
        return touch.phase == TouchPhase.Ended;
#endif
    }

    IEnumerator RemoveColliderTailCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(colliderPopStepTime);
            if (collider.pointCount > 0)
                RemoveColliderTail();
        }
    }
    //IEnumerator RemoveLineTailCoroutine()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(linePopStepTime);

    //        if (i > 0)
    //            RemoveLineTail();
    //    }
    //}

    //void RemoveLineTail()
    //{
    //    //remove the first pt in line
    //    var positions = new Vector3[line.positionCount];
    //    line.GetPositions(positions);
    //    var list = positions.ToList();
    //    list.RemoveAt(0);

    //    i = list.Count;
    //    line.positionCount = i;
    //    line.SetPositions(list.ToArray());

    //    if (line.positionCount <= 0)
    //    {
    //        line.enabled = false;
    //    }
    //}
    void RemoveColliderTail()
    {
        //se sono 2 devo metterli nello stesso punto in modo da non avere code
        if (collider.pointCount <= 2)
        {
            collider.enabled = false;
            return;
        }

        var pts = collider.points.ToList();
        pts.RemoveAt(0);

        collider.points = pts.ToArray();
    }
    //void EmptyLine()
    //{
    //    line.positionCount = 0;
    //    line.SetPositions(new Vector3[] { });
    //}
    void EmptyCollider()
    {
        collider.points = new Vector2[] { Vector2.zero, Vector2.zero };
    }
    void StartAllCoroutines()
    {
        if (colliderCoroutine == null)
        {
            colliderCoroutine = RemoveColliderTailCoroutine();
            StartCoroutine(colliderCoroutine);
        }
        //if (lineCoroutine == null)
        //{
        //    lineCoroutine = RemoveLineTailCoroutine();
        //    StartCoroutine(lineCoroutine);
        //}
    }
    new void StopAllCoroutines()
    {
        if (colliderCoroutine != null)
            StopCoroutine(colliderCoroutine);
        colliderCoroutine = null;

        //if (lineCoroutine != null)
        //    StopCoroutine(lineCoroutine);
        //lineCoroutine = null;
        //i = 0;
    }
    //bool CanAddLineVertex(Vector3 newPos)
    //{
    //    if (line.positionCount <= 0) return true;
    //    return Vector3.Distance(line.GetPosition(line.positionCount - 1), newPos) > collidersTreshold;
    //}
    void Update()
    {
        if (TouchAvailable())
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = -5;

            if (TouchEnding())
            {
                //line.positionCount = 0;
                //line.enabled = false;
                //i = 0;

                //remove col
                collider.enabled = false;

                //StopAllCoroutines();

            }
            else if (TouchStarting())
            {
                //StartAllCoroutines();
                collider.enabled = true;

                collider.enabled = true;
                //line.enabled = true;

                Vector2 nextPos = transform.InverseTransformPoint(worldPos);
                collider.points = new Vector2[] { nextPos, nextPos };

                //line.positionCount = 0;
                //i = 0;
                //line.SetPositions(new Vector3[] { });

                currentTrail.SetActive(false);
                currentTrail.transform.position = worldPos;
                currentTrail.SetActive(true);
            }
            else if (TouchMoving())
            {
                //line.enabled = false;

                //Vector3 dir = (worldPos - currentTrail.transform.position).normalized;
                //dir.z = 0;
                //currentTrail.transform.forward = dir;
                currentTrail.transform.position = worldPos;

                //if (i + 1 > maxLinePointsAllowed)
                //{
                //    RemoveLineTail();
                //}

                if (collider.points.Length > maxColliderPointsAllowed)
                {
                    RemoveColliderTail();
                }

                //if (CanAddLineVertex(worldPos))
                //{
                //    line.positionCount = i + 1;
                //    line.SetPosition(i, worldPos);
                //    i++;

                //    //line.enabled = true;
                //}

                //add collider 
                Vector2 nextPos = transform.InverseTransformPoint(worldPos);
                if (collider.pointCount > 0)
                {
                    var distance = nextPos - collider.points[collider.pointCount - 1];
                    if (distance.magnitude < collidersTreshold) return;
                }

                var newPts = collider.points.ToList();
                newPts.Add(nextPos);
                collider.points = newPts.ToArray();
                collider.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var c = col.GetComponent<SmashableObject>();
        if (c != null)
        {
            c.Smash();
        }
    }
}
