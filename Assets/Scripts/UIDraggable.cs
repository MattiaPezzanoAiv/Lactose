using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IDragHandler
{
    RectTransform tr;

    public void OnDrag(PointerEventData eventData)
    {
        if (tr == null)
            tr = GetComponent<RectTransform>();

        //Debug.Log("position " + fixedPos);
        //Debug.Log("screen " + new Vector2(Screen.width, Screen.height));

        Vector2 deltas = new Vector2(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
        Vector2 fixedPos = new Vector2(Mathf.Lerp(0, 1920, deltas.x),Mathf.Lerp(0,1080, deltas.y));

        if (fixedPos.x > 1920)
            fixedPos.x = 1920;
        if (fixedPos.x < 0)
            fixedPos.x = 0;
        if (fixedPos.y > 1080)
            fixedPos.y = 1080;
        if (fixedPos.y < 0)
            fixedPos.y = 0;

        tr.anchoredPosition = fixedPos;
    }
}
