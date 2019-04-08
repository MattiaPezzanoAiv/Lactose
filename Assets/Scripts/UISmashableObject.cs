using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UISmashableObject))]
public class UISmashableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Smash"))
        {
            (target as UISmashableObject).Smash();
        }
        base.OnInspectorGUI();
    }
}
#endif


public class UISmashableObject : SmashableObject
{
    public static bool canClick = true;

    public RectTransform rootForSliced;
    public UnityEngine.UI.Image gfx;
    [SerializeField]
    float eventThrowDelay = 2f;

    public UnityEvent onSmashEvent;

    private UISlicedObject sliced;

    private void Reset()
    {
        StopAllCoroutines();

        if (sliced != null)
            Destroy(sliced);

        canClick = true;
    }

    protected override void Awake()
    {
        //must be overrided and empty
    }
    public override void Smash(bool addPts = true)
    {
        if (!canClick) return;

        gfx.enabled = false;
        canClick = false;
        GameManager.Instance.StartCoroutine(ThrowDelayed());

        if (sliced != null)
            Destroy(sliced);

        sliced = (UISlicedObject)Instantiate(slicedPrefab, rootForSliced);
        sliced.gameObject.layer = LayerMask.NameToLayer("UI");
        sliced.transform.position = transform.position;

        foreach (var r in sliced.GetComponentsInChildren<Rigidbody2D>())
        {
            r.gravityScale = 1;// Screen.height * gravityPercentageOfScreen / 100f;
        }
        sliced.Slice(slicedRight, slicedLeft, new Vector2(0f, 1f), 2.5f, 4f);

        var effect = Instantiate(this.splashPrefab, this.transform.position, Quaternion.identity);
        Destroy(effect, 3f);
    }
    
    IEnumerator ThrowDelayed()
    {
        yield return new WaitForSeconds(eventThrowDelay);

        if (onSmashEvent != null)
            onSmashEvent.Invoke();

        yield return new WaitForSeconds(1f);

        canClick = true;
        gfx.enabled = true;
    }

}
