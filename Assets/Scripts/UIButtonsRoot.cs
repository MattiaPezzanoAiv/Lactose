using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonsRoot : MonoBehaviour, IDragHandler
{
    [SerializeField] List<GameObject> buttons;

    public Vector2 randomIntervalBetweenBounce;

    private List<Animation> animations;

    void Awake()
    {
        animations = new List<Animation>();
        foreach (var b in buttons)
        {
            var a = b.transform.GetComponentInParent<Animation>();
            a.clip.legacy = true;
            a.clip.wrapMode = WrapMode.Once;
            animations.Add(a);
        }
    }
    
    void OnEnable()
    {
        StartCoroutine(BounceCoroutine());
    }
    void OnDisable()
    {
        StopCoroutine(BounceCoroutine());
    }
    IEnumerator BounceCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(randomIntervalBetweenBounce.x, randomIntervalBetweenBounce.y));
            var a = animations[Random.Range(0, animations.Count)];
            a.Rewind();
            a.Play();

            yield return new WaitForSeconds(a.clip.length);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
        if (buttons.Contains(eventData.pointerEnter))
        {
            int idx = buttons.IndexOf(eventData.pointerEnter);
            buttons[idx].transform.parent.GetComponent<UISmashableObject>().Smash();
        }
    }
}
