using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMainPage : MonoBehaviour
{
    public RectTransform mainRoot;
    public RectTransform othersRoot;
    public GameObject blockInput;

    public TextMeshProUGUI bestScore;
    public Text tfVersion;

    public bool useRandomCurveForAnimation;
    public AnimationCurve[] curvesX;
    public AnimationCurve[] curvesY;

    private bool isX;

    AnimationCurve CurveX
    {
        get
        {
            isX = true;
            return curvesX[Random.Range(0, curvesX.Length)];
        }
    }
    AnimationCurve CurveY
    {
        get
        {
            isX = false;
            return curvesY[Random.Range(0, curvesY.Length)];
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            bestScore.text = "Your best:\n" + GameManager.Instance.GetBestScore();
    }


    // Start is called before the first frame update
    void Start()
    {
        tfVersion.text = GameManager.GAME_VERSION;
        mainRoot.gameObject.SetActive(true);
        othersRoot.gameObject.SetActive(false);
        blockInput.SetActive(false);

        bestScore.text = "Your best:\n" + GameManager.Instance.GetBestScore();
    }

    public void ShowMain()
    {
        if (Random.Range(0f, 1f) > 0.5f)
        {
            StartCoroutine(Animate(othersRoot, mainRoot));
            return;
        }
        StartCoroutine(AnimateScale(othersRoot, mainRoot));
    }
    public void ShowOther()
    {
        if (Random.Range(0f, 1f) > 0.5f)
        {
            StartCoroutine(Animate(mainRoot, othersRoot));
            return;
        }
        StartCoroutine(AnimateScale(mainRoot, othersRoot));
    }
    IEnumerator AnimateScale(RectTransform exiting, RectTransform entering)
    {
        blockInput.SetActive(true);

        for (float i = 0; i < 0.3; i += Time.deltaTime)
        {
            //exiting
            float val = 1 - (i / 0.3f);
            exiting.localScale = new Vector3(val, val, val);
            yield return null;
        }

        exiting.localScale = new Vector3(0, 0, 0);
        exiting.gameObject.SetActive(false);
        entering.gameObject.SetActive(true);

        for (float i = 0; i < 0.3; i += Time.deltaTime)
        {
            //exiting
            float val = i / 0.3f;
            entering.localScale = new Vector3(val, val, val);
            yield return null;
        }

        entering.localScale = Vector3.one;
        exiting.localScale = Vector3.one;
        blockInput.SetActive(false);
    }
    IEnumerator Animate(RectTransform exiting, RectTransform entering, AnimationCurve animCurve = null)
    {
        if (animCurve == null || useRandomCurveForAnimation)
        {
            animCurve = Random.Range(0f, 1f) > 0.5f ? CurveX : CurveY;
        }


        blockInput.SetActive(true);

        float time = animCurve.keys[animCurve.keys.Length - 1].time;

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            //exiting
            Vector2 pos = exiting.anchoredPosition;
            pos.x = isX ? animCurve.Evaluate(i) : pos.x;
            pos.y = isX ? pos.y : animCurve.Evaluate(i);

            exiting.anchoredPosition = pos;
            yield return null;
        }

        exiting.gameObject.SetActive(false);
        entering.gameObject.SetActive(true);

        for (float i = time; i > 0; i -= Time.deltaTime)
        {
            //entering
            Vector2 pos = exiting.anchoredPosition;
            pos.x = isX ? animCurve.Evaluate(i) : pos.x;
            pos.y = isX ? pos.y : animCurve.Evaluate(i);

            entering.anchoredPosition = pos;
            yield return null;
        }

        entering.anchoredPosition = Vector3.zero;
        exiting.anchoredPosition = Vector3.zero;
        blockInput.SetActive(false);
    }
}
