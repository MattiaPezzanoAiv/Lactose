using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievmentsPage : MonoBehaviour
{
    public Text tfDescr;
    public RectTransform balloon;
    public Vector2 startPos, endPos;
    public float stayTime = 1.5f;

    Queue<IEnumerator> coroutines;
    Coroutine currentCor;

    private void Awake()
    {
        coroutines = new Queue<IEnumerator>();
    }

    /// <summary>
    /// return the delay time to wait for animation end
    /// </summary>
    /// <param name="descr"></param>
    /// <returns></returns>
    public float PlayAnim(string descr)
    {
        if(coroutines.Count <= 0 && currentCor == null) //none playing
        {
            currentCor = StartCoroutine(PlayAnimCoroutine(descr));
            return 1f + stayTime;
        }

        coroutines.Enqueue(PlayAnimCoroutine(descr));
        
        return 1f + stayTime;
    }
    private void Update()
    {
        if (currentCor == null && coroutines.Count > 0)
        {
            currentCor = StartCoroutine(coroutines.Dequeue());
        }
        else if (currentCor == null && coroutines.Count <= 0)
            UIManager.Instance.HideInstant("UIAchievementUnlocked");

    }
    IEnumerator PlayAnimCoroutine(string descr)
    {
        yield return null;

        tfDescr.text = descr;
        balloon.anchoredPosition = startPos;
        int dir = -1;
        for (float i = 0; i < 0.5f; i+= Time.deltaTime)
        {
            float delta = i / 0.5f;
            balloon.anchoredPosition = Vector2.Lerp(startPos, endPos, delta);
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        dir *= -1;
        for (float i = 0; i < 0.5f; i += Time.deltaTime)
        {
            float delta = i / 0.5f;
            balloon.anchoredPosition = Vector2.Lerp(endPos, startPos, delta);
            yield return null;
        }

        balloon.anchoredPosition = startPos;

        currentCor = null;
    }
}
