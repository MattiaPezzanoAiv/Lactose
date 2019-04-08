using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ParallaxMapping))]
public class MappingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Reset"))
        {
            ParallaxMapping.Instance.Play(true);
        }

        base.OnInspectorGUI();
    }
}
#endif

public class ParallaxMapping : MonoBehaviour
{
    public static ParallaxMapping Instance { get; private set; }

    [System.Serializable]
    class ParallaxSettings
    {
        public Sprite sprite;
        public Vector2 offset;
        public float speed;
        public int order;
        public bool fitScreen;

        [Header("props")]
        public bool useSin;
        public float sinPower;
        public float sinSpeed;
    }

    [SerializeField]
    private float parallaxCorrectionDelta = 0f;
    [SerializeField]
    private ParallaxSettings[] layers;

    private Dictionary<ParallaxSettings, List<SpriteRenderer>> map;
    private int idx = 0;

    float leftX, rightX, worldWidth;

    bool paused = false;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        map = new Dictionary<ParallaxSettings, List<SpriteRenderer>>();
        transform.position = Vector3.zero;
        foreach (var s in layers)
        {
            Create(s);
        }

        Pause();

        leftX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        rightX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        worldWidth = rightX - leftX;
    }
    private void Start()
    {
        GameManager.Instance.onMatchLostBefore += () =>
        {
            this.Pause();
        };
    }


    void SetupPositions(GameObject go1, GameObject go2, Vector3 offset)
    {
        Vector3 leftPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 rightPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));

        leftPos.z = 0;
        rightPos.z = 0;

        go1.transform.localPosition = leftPos + offset;
        go2.transform.localPosition = rightPos + offset;
    }
    void Create(ParallaxSettings layer)
    {
        map.Add(layer, new List<SpriteRenderer>());

        //first
        var go = new GameObject();
        go.transform.SetParent(transform);

        var rend = go.AddComponent<SpriteRenderer>();
        map[layer].Add(rend);

        rend.sprite = layer.sprite;
        rend.sortingOrder = layer.order;


        //second
        var go2 = new GameObject();
        go2.transform.SetParent(transform);

        var rend2 = go2.AddComponent<SpriteRenderer>();
        map[layer].Add(rend2);

        rend2.sprite = layer.sprite;
        rend2.sortingOrder = layer.order;

        //setup positions
        SetupPositions(go, go2, layer.offset);

        if (!layer.fitScreen) return;

        //fit screen
        var width = rend.sprite.bounds.size.x;
        var height = rend.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 scale = rend.transform.localScale;
        scale.x = (worldScreenWidth / width) + 0.005f;
        scale.y = worldScreenHeight / height;

        rend.transform.localScale = scale;
        rend2.transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused) return;
        
        foreach (var i in map)
        {
            for (int j = 0; j < i.Value.Count; j+=2)
            {
                var bg = i.Value[j];
                var bg2 = i.Value[j + 1];
                MoveBG(bg, i.Key);
                MoveBG(bg2, i.Key);
            }
        }
    }
    
    void MoveBG(SpriteRenderer bg, ParallaxSettings settings)
    {
        var pos = bg.transform.localPosition;
        pos.x += -1 * settings.speed * Time.deltaTime;
        pos.y += settings.useSin ? Mathf.Sin(Time.time * settings.sinPower) * settings.sinSpeed * Time.deltaTime : 0f;

        bg.transform.localPosition = pos;

        if (bg.transform.position.x + worldWidth < leftX)
        {
            if (settings.fitScreen)
            {
                var spriteWidth = bg.sprite.bounds.size.x * bg.transform.localScale.x;

                var p = bg.transform.position;
                p.x = rightX;//(spriteWidth * 2f);
                bg.transform.position = p;
            }
            else
            {
                var p = bg.transform.position;
                p.x = rightX + settings.offset.x;
                bg.transform.position = p;
            }
        }
    }

    public void Play(bool reset)
    {
        paused = false;

        if(reset)
        {
            foreach (var m in map)
            {
                SetupPositions(m.Value[0].gameObject, m.Value[1].gameObject, m.Key.offset);
            }
        }
    }
    public void Pause()
    {
        paused = true;
    }
}
