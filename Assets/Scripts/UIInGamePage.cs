using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInGamePage : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image pauseGfx;
    [SerializeField]
    Sprite paused, unPaused;
    [SerializeField]
    public UnityEngine.UI.Text bestScoreText, currentScoreText, newBestScoreText;
    [SerializeField] GameObject newBestScoreGo;
    [SerializeField]
    UIBar sicknessBar;

    Vector3 newBestStartPosition;

    void UpdateGfx()
    {
        pauseGfx.sprite = GameManager.Instance.IsPaused ? paused : unPaused;
    }

    private void Start()
    {
        sicknessBar.Setup(GameManager.Instance.maxHerSickness, 0, GameManager.Instance.maxHerSicknessPixels);

        GameManager.Instance.onMatchPause += UpdateGfx;
        GameManager.Instance.onMatchResume += UpdateGfx;

        //register to points changed events (devi setuppare la barra della sickness, prendi reference)
        GameManager.Instance.onPointsChanged += (newPts, shake) =>
        {
            bestScoreText.text = "Best Score: " + GameManager.Instance.GetBestScore();
            currentScoreText.text = "Score: " + newPts;
        };
        GameManager.Instance.onHerSicknessChanged += (sick, shake) =>
        {
            sicknessBar.Refresh(sick, shake);
        };

        GameManager.Instance.onFirstNewBestScoreInMatch += OnNewBestScore;

        GameManager.Instance.onFirstNewBestScoreInMatchEnd += () =>
        {
            newBestScoreGo.SetActive(false);
            StopAllCoroutines();
        };

        //setup current ui
        bestScoreText.text = "Best Score: " + GameManager.Instance.GetBestScore();
        currentScoreText.text = "Score: " + 0;
    }


    void OnNewBestScore(int score)
    {
        StartCoroutine(AnimateNewBestScore(score));
        StartCoroutine(VibrateNewBestScore());
    }
    IEnumerator VibrateNewBestScore()
    {
        newBestStartPosition = newBestScoreGo.transform.position;
        while (true)
        {
            Vector3 p = Random.insideUnitCircle * 0.3f;
            newBestScoreGo.transform.position = Vector3.Lerp(newBestStartPosition, newBestStartPosition + p, 0.5f);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    IEnumerator AnimateNewBestScore(int score)
    {
        newBestScoreGo.SetActive(true);
        newBestScoreGo.transform.localScale = Vector3.zero;
        newBestScoreText.text = score.ToString();

        AnimationCurve curve = new AnimationCurve();
        curve.keys = new Keyframe[]
        {
            new Keyframe(0,0),
            new Keyframe(0.5f, 1.3f),
            new Keyframe(0.7f, 1f)
        };

        for (float i = 0; i < 0.7f; i += Time.unscaledDeltaTime)
        {
            float val = curve.Evaluate(i);
            newBestScoreGo.transform.localScale = new Vector3(val, val, val);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        newBestScoreGo.transform.localScale = Vector3.one;
    }
}
