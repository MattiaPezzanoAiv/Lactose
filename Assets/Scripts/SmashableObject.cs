using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashableObject : MonoBehaviour
{
    [SerializeField]
    protected GameObject splashPrefab;

    [SerializeField]
    private bool isSoy, sliceOnDestroy = true;
    [SerializeField]
    protected SlicedObject slicedPrefab;
    [SerializeField]
    protected Sprite slicedLeft, slicedRight;

    public WaveLevel[] availableLevels;

    private bool smashed = false;
    public Vector2 PausedVelocity { get; protected set; }
    public bool IsSoy { get { return isSoy; } }

    public UnityEngine.Events.UnityAction onSmash;

    public void Pause()
    {
        var body = GetComponent<Rigidbody2D>();
        PausedVelocity = body.velocity;
        body.velocity = Vector2.zero;
        body.gravityScale = 0f;
    }
    public void Resume()
    {
        var body = GetComponent<Rigidbody2D>();
        body.velocity = PausedVelocity;
        body.gravityScale = 1f;
    }

    public virtual void Throw(Vector2 velocity)
    {
        GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);
    }

    public virtual void Smash(bool addPts = true)
    {
        if (onSmash != null)
            onSmash.Invoke();

        if(addPts)
        {
            if (!isSoy)
                GameManager.Instance.AddPts(GameManager.Instance.ptsPerObject, transform.position);
            else
                GameManager.Instance.HerSickness += GameManager.Instance.sicknessPerObject; //to be param
        }

        //slice or splash
        smashed = true;

        if(sliceOnDestroy)
        {
            var obj = Instantiate(slicedPrefab, transform.position, Quaternion.identity);
            obj.Slice(slicedRight, slicedLeft, GetComponent<Rigidbody2D>().velocity, 2.5f, 4f);
        }
        else
        {
            //instantiate splash
        }

        var splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);
        Destroy(splash, 4f);
        Destroy(gameObject);
    }

    public void RemoveForEndMatch(float delay = -1f)
    {
        smashed = true;
        if (delay <= 0f)
            Destroy(gameObject);
        else
            Destroy(gameObject, delay);
    }
    public void RemoveForEndMatchSmashed(float delay = -1f)
    {
        Smash(false);
        //if (delay <= 0f)
        //    Destroy(gameObject);
        //else
        //    Destroy(gameObject, delay);
    }

    protected virtual void OnBecameInvisible()
    {
        if (isQuitting) return;
        if (smashed) return;

#if UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TEST")
            return;
#endif


        //Debug.LogError("invisible");
        if (!isSoy)
            GameManager.Instance.HerSickness += GameManager.Instance.sicknessPerObject;
        else
            GameManager.Instance.HerSickness -= GameManager.Instance.sicknessPerObject;

        Destroy(gameObject);
    }

    private bool isQuitting;
    protected virtual void Awake()
    {
        Application.quitting += () => isQuitting = true;
    }
}
