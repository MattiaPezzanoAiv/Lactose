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
    public AnimationCurve[] curves;

    AnimationCurve Curve
    {
        get
        {
            return curves[Random.Range(0, curves.Length)];
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
        StartCoroutine(Animate(othersRoot, mainRoot));
    }
    public void ShowOther()
    {
        StartCoroutine(Animate(mainRoot, othersRoot));
    }
    IEnumerator Animate(RectTransform exiting, RectTransform entering, AnimationCurve animCurve = null)
    {
        if(animCurve == null || useRandomCurveForAnimation)
        {
            animCurve = Curve;
        }

        blockInput.SetActive(true);

        float time = animCurve.keys[animCurve.keys.Length - 1].time;

        for (float i = 0; i < time; i+= Time.deltaTime)
        {
            //exiting
            exiting.anchoredPosition = new Vector2(animCurve.Evaluate(i), exiting.anchoredPosition.y);
            yield return null;
        }

        exiting.gameObject.SetActive(false);
        entering.gameObject.SetActive(true);

        for (float i = time; i > 0; i -= Time.deltaTime)
        {
            //entering
            entering.anchoredPosition = new Vector2(animCurve.Evaluate(i), exiting.anchoredPosition.y);
            yield return null;
        }
        blockInput.SetActive(false);
    }
}
