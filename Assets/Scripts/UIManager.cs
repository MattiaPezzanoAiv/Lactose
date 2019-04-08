using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{

    public interface IBackButtonHandler
    {
        void OnBackButtonReleased();
    }

    public float fadeDuration;

    public List<GameObject> pages;
    private Dictionary<string, GameObject> _pages;

    private GameObject activePage;

    private void Awake()
    {
        Instance = this;
        _pages = new Dictionary<string, GameObject>();
        foreach (var p in pages)
        {
            _pages.Add(p.name, p);
        }
    }

    private void ShowInternal(string name)
    {
        if (fadeC != null)
            return; //cant start a fade cuz another one alredy running. nothing will happen

        StartCoroutine(ShowCoroutine(name));
    }
    private IEnumerator ShowCoroutine(string name)
    {
        FadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        HideAll();
        _pages[name].SetActive(true);
        activePage = _pages[name];

        FadeIn(fadeDuration);
    }
    private void HideAll()
    {
        foreach (var p in pages)
        {
            if (p.name == "UIAchievementUnlocked")
                continue;
            p.SetActive(false);
        }
    }
    public void Show(string name)
    {
        if (!_pages.ContainsKey(name))
        {
            Debug.LogError("Page not found => " + name);
            return;
        }
        ShowInternal(name);
    }

    public void OnBackButtonReleased()
    {
        if (activePage == null) return;

        IBackButtonHandler handler = activePage.GetComponent<IBackButtonHandler>();
        if (handler == null) return;

        handler.OnBackButtonReleased();
    }

    public void ShowInstant(string name)
    {
        if (!_pages.ContainsKey(name))
        {
            Debug.LogError("Page not found => " + name);
            return;
        }
        _pages[name].SetActive(true);
    }
    public void HideInstant(string name)
    {
        if (!_pages.ContainsKey(name))
        {
            Debug.LogError("Page not found => " + name);
            return;
        }
        _pages[name].SetActive(false);
    }
    public T Get<T>(string name) where T : MonoBehaviour
    {
        if (!_pages.ContainsKey(name))
        {
            Debug.LogError("Page not found => " + name);
            return null;
        }
        return _pages[name].GetComponent<T>();
    }
    public GameObject Get(string name)
    {
        if (!_pages.ContainsKey(name))
        {
            Debug.LogError("Page not found => " + name);
            return null;
        }
        return _pages[name];
    }

    Coroutine fadeC;

    public void FadeIn(float time)
    {
        if (fadeC != null)
            StopCoroutine(fadeC);

        fadeC = StartCoroutine(FadeCoroutine(time, true));
    }
    public void FadeOut(float time)
    {
        if (fadeC != null)
            StopCoroutine(fadeC);

        fadeC = StartCoroutine(FadeCoroutine(time, false));
    }
    IEnumerator FadeCoroutine(float time, bool isFadeIn)
    {
        var fader = Get("UIFader").GetComponentInChildren<UnityEngine.UI.Image>();
        ShowInstant("UIFader");

        float startAlpha = isFadeIn ? 1f : 0f;
        float endAlpha = isFadeIn ? 0f : 1f;

        //set start alpha
        Color c = fader.color;
        c.a = startAlpha;
        fader.color = c;

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            float delta = i / time;
            c = fader.color;
            c.a = Mathf.Lerp(startAlpha, endAlpha, delta);
            fader.color = c;
            yield return null;
        }

        c = fader.color;
        c.a = endAlpha;
        fader.color = c;
        HideInstant("UIFader");

        fadeC = null;
    }
}
