using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

//id applicazione 1014223040299

public class GameManager : SingletonBehaviour<GameManager>
{
#if UNITY_EDITOR
    //fai pulsante per resettare la prefs dei punti e testa il locked delle trail+ fai animazione per la trail
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Reset Prefs"))
            {
                PlayerPrefs.DeleteAll();
            }
            if (GUILayout.Button("Fireworks"))
            {
                var me = (target as GameManager);
                me.ThrowFireworks(0.7f);
            }

            base.OnInspectorGUI();
        }
    }
#endif
    public const string GAME_VERSION = "V0.1.16";
    public const string RELEASE_NOTES = "Added google services";
    public const string GLOBAL_PTS_COUNTER = "GLOBAL_PTS_COUNTER";

    private void Awake()
    {
        Instance = this;

        onMatchLostAfter += StopMatch;

        fireworks = new List<ParticleSystem>();
        for (int i = 0; i < fireworksRoot.transform.childCount; i++)
            fireworks.Add(fireworksRoot.transform.GetChild(i).GetComponent<ParticleSystem>());

        DisableFireworks();

        _internalGlobalPtsCounter = PlayerPrefs.GetInt(GLOBAL_PTS_COUNTER, 0);
    }

    private void Start()
    {
        //UIManager.Instance.HideAll();
        onStreakBonus += (count, sourcePos) =>
        {
            var txt = Instantiate(streakBonusTextPrefab, sourcePos, Quaternion.identity);
            txt.GetComponentInChildren<TextMeshPro>().text = "Streak! x" + count;
            txt.clip.legacy = true;
            txt.clip.wrapMode = WrapMode.Once;
            txt.Play();

            Destroy(txt.gameObject, 5f);
        };

        UIManager.Instance.ShowInstant("UIMainPage");
        //UIManager.Instance.HideInstant("UIFader");

        UIManager.Instance.FadeIn(0.5f);

#if M_DEBUG
        //UIManager.Instance.ShowInstant("UIDebugPage");
#endif

        HerSickness = 0;
        //onHerSicknessChanged.Invoke(HerSickness);
        //UIPtsUpdater.Instance.InitUI(maxHerSickness, maxHerSicknessPixels);
    }

    public Animation streakBonusTextPrefab;

    public GameObject fireworksRoot;
    public float fireworksInterval;
    private List<ParticleSystem> fireworks;

    [SerializeField]
    private string bestScoreKey;
    
    public int sicknessPerObject, ptsPerObject;

    public int maxHerSickness, maxHerSicknessPixels, minSmashForBonus = 3;
    public float maxTimeForBonus = 0.5f, delayAfterDead = 1.5f;

    private bool shouldCheckForFirstBestScore;
    private int pts, herSickness, smashStreak, _internalGlobalPtsCounter;
    private Vector3 lastSourcePosition;
    private Coroutine checkStreakCoroutine;

    [HideInInspector]
    public UnityEngine.Events.UnityAction onMatchStart, onMatchPause, onMatchResume, onFirstNewBestScoreInMatchEnd, onMatchLostBefore;
    [HideInInspector]
    public UnityEngine.Events.UnityAction<int> onFirstNewBestScoreInMatch;
    [HideInInspector]
    public UnityEngine.Events.UnityAction<bool, bool> onMatchLostAfter;
    [HideInInspector]
    public UnityEngine.Events.UnityAction<int, bool> onPointsChanged, onHerSicknessChanged;
    //bool is allow shake
    [HideInInspector]
    public UnityEngine.Events.UnityAction<int, Vector3> onStreakBonus;   //streakcount, laststreakobject

    public bool IsPaused { get; private set; }
    public bool ShouldShakeSicknessBar { get; set; }

    public void ShowAchievs()
    {
        UIManager.Instance.Show("UIAchiListPage");
    }
    public void ShowCredits()
    {
        //UIManager.Instance.HideAll();
        UIManager.Instance.Show("UICreditsPage");
    }
    public void ShowCustomizePage()
    {
        UIManager.Instance.Show("UICustomizePage");
    }


    #region FIREWORKS
    void DisableFireworks()
    {
        foreach (var f in fireworks)
        {
            f.gameObject.SetActive(false);
        }
        fireworksCoroutine = null;
    }
    public void ThrowFireworks(float interval)
    {
        if (fireworksCoroutine != null) return;

        fireworksCoroutine = StartCoroutine(ThrowFireworksCoroutine(interval));
    }
    Coroutine fireworksCoroutine;
    IEnumerator ThrowFireworksCoroutine(float interval)
    {
        foreach (var f in fireworks)
        {
            f.gameObject.SetActive(true);
            f.Play();
            yield return new WaitForSecondsRealtime(interval);
        }

        yield return new WaitForSecondsRealtime(3f);
        DisableFireworks();
    }
    #endregion
    #region BEST_SCORE
    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(bestScoreKey, 0);
    }
    public void SetBestScore(int newScore)
    {
        if (ShouldSaveBestScore(newScore))
        {
            PlayerPrefs.SetInt(bestScoreKey, newScore);
            SocialManager.Implementation.AddScoreToLeaderboard(newScore);
        }
    }
    public bool ShouldSaveBestScore(int newScore)
    {
        return (GetBestScore() < newScore);
    }
    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(GLOBAL_PTS_COUNTER, _internalGlobalPtsCounter);
    }
    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetInt(GLOBAL_PTS_COUNTER, _internalGlobalPtsCounter);

        
    }
    private void Update()
    {
#if UNITY_EDITOR || UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UIManager.Instance.OnBackButtonReleased();
        }
#endif
    }
    #region MATCH
    public void QuitMatch()
    {
        SetBestScore(GetPoints());

        ResumeMatch();  //for time scale

        HerSickness = 0;
        ResetPoints();
        //onHerSicknessChanged.Invoke(HerSickness);
        //onPointsChanged.Invoke(pts);
        //UIPtsUpdater.Instance.UpdateUI(Pts, HerSickness);

        ParallaxMapping.Instance.Pause();

        StopAllCoroutines();

        //is for quit and no game over should be showed
        if (onMatchLostBefore != null)
            onMatchLostBefore.Invoke();
        if (onMatchLostAfter != null)
            onMatchLostAfter.Invoke(false, false);

        //UIManager.Instance.HideAll();
        //UIManager.Instance.Show("UIMainPage");
#if M_DEBUG
        UIManager.Instance.Show("UIMainPage");
#else
        UIManager.Instance.Show("UIMainPage");
#endif
    }

    public void StopMatch(bool isGameOver, bool isNewRecord)
    {
        ParallaxMapping.Instance.Pause();

        if (isGameOver)
        {
            UIManager.Instance.ShowInstant("UIGameOverPage");
            var page = UIManager.Instance.Get<UIGameOverPage>("UIGameOverPage");


            if (isNewRecord)
                page.SetupNewRecord(GetBestScore());
            else
                page.SetupLose();
        }
        HerSickness = 0;
        ResetPoints();
        UIManager.Instance.HideInstant("UIInGamePage");
#if M_DEBUG
        //UIManager.Instance.ShowInstant("UIDebugPage");
        //UIDebugPage.Instance.gameObject.SetActive(false);
#endif
    }
    void PauseMatch(bool showPage = true)
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (showPage)
            UIManager.Instance.ShowInstant("UIPausePage");
        //inGamePage.SetActive(false);

        if (onMatchPause != null)
            onMatchPause.Invoke();
    }
    void ResumeMatch()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.HideInstant("UIPausePage");
        //inGamePage.SetActive(true);

        if (onMatchResume != null)
            onMatchResume.Invoke();
    }
    public void TogglePause()
    {
        if (IsPaused)
            ResumeMatch();
        else
            PauseMatch();
    }
    public void StartMatchTutorial()
    {
        TutorialManager.Instance.StartTutorial();
    }
    public void StartMatch()
    {
        if (GetBestScore() > 0)
            shouldCheckForFirstBestScore = true;
        else shouldCheckForFirstBestScore = false;

        if (onMatchStart != null)
            onMatchStart.Invoke();

        ParallaxMapping.Instance.Play(true);

        //UIManager.Instance.HideAll();
        if (wasGameOver)
        {
            UIManager.Instance.ShowInstant("UIInGamePage");
            UIManager.Instance.HideInstant("UIGameOverPage");
        }
        else
            UIManager.Instance.Show("UIInGamePage");

        wasGameOver = false;

#if M_DEBUG
        //UIDebugPage.Instance.gameObject.SetActive(true);
#endif
    }
    #endregion


    #region PROPERTY
    void StopAndThrowLostEvent(bool isGameOver, bool isNewRecord)
    {
        StopAllCoroutines();

        if (onMatchLostAfter != null)
            onMatchLostAfter.Invoke(isGameOver, isNewRecord);
    }

    private bool wasGameOver = false;
    public void NotGameOver()
    {
        wasGameOver = false;
    }

    public void ResetPoints()
    {
        pts = 0;
        smashStreak = 0;

        if (onPointsChanged != null)
            onPointsChanged.Invoke(pts, false);

        if (checkStreakCoroutine != null)
        {
            StopCoroutine(checkStreakCoroutine);
            checkStreakCoroutine = null;
        }
    }
    public void AddPts(int amount, Vector3 source)
    {
        smashStreak++;
        int multiplier = smashStreak;

        lastSourcePosition = source;
        if (checkStreakCoroutine == null)
            checkStreakCoroutine = StartCoroutine(CheckBonusCoroutine());

        bool wasStreak = false;
        if (smashStreak >= minSmashForBonus)
        {
            smashStreak = 0;
            wasStreak = true;
            //elegible for bonus
            if (onStreakBonus != null)
                onStreakBonus.Invoke(multiplier, lastSourcePosition);

            if (checkStreakCoroutine != null)
            {
                StopCoroutine(checkStreakCoroutine);
                checkStreakCoroutine = StartCoroutine(CheckBonusCoroutine());
            }
        }

        pts += (amount * (wasStreak ? multiplier : 1));

        if (shouldCheckForFirstBestScore && pts > GetBestScore())
        {
            SetBestScore(pts);
            StartCoroutine(FirstBestScoreCoroutine());
            shouldCheckForFirstBestScore = false;
            if (onFirstNewBestScoreInMatch != null)
                onFirstNewBestScoreInMatch.Invoke(pts);
        }

        if (pts >= 10000)
            AchievmentsManager.Instance.Unlock<Record10k>();
        if (pts >= 25000)
            AchievmentsManager.Instance.Unlock<Record25k>();
        if (pts >= 50000)
            AchievmentsManager.Instance.Unlock<Record50k>();

        _internalGlobalPtsCounter += (amount * (wasStreak ? multiplier : 1));

        if (_internalGlobalPtsCounter >= 100000)
            AchievmentsManager.Instance.Unlock<Collect100kPoints>();
        if (_internalGlobalPtsCounter >= 200000)
            AchievmentsManager.Instance.Unlock<Collect200kPoints>();
        if (_internalGlobalPtsCounter >= 500000)
            AchievmentsManager.Instance.Unlock<Collect500kPoints>();


        if (onPointsChanged != null)
            onPointsChanged.Invoke(pts, false);
    }
    IEnumerator CheckBonusCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(maxTimeForBonus);
            smashStreak = 0;
        }
    }
    IEnumerator FirstBestScoreCoroutine()
    {
        this.PauseMatch(false);

        if (fireworks != null)
        {
            ThrowFireworks(fireworksInterval);   //automatically disabled
        }

        //play anim of character
        //show new record text

        yield return new WaitForSecondsRealtime(2f);

        //play her normal animation

        if (onFirstNewBestScoreInMatchEnd != null)
            onFirstNewBestScoreInMatchEnd.Invoke();
        this.ResumeMatch();
    }

    public int GetPoints()
    {
        return pts;
    }

    public int HerSickness
    {
        get { return herSickness; }
        set
        {
            bool shake = true;
            if (value - herSickness > sicknessPerObject || value - herSickness < 0)
            {
                shake = false;
            }


            herSickness = value;

            if (herSickness > maxHerSickness)
                herSickness = maxHerSickness;

            if (onHerSicknessChanged != null)
                onHerSicknessChanged.Invoke(herSickness, shake);

            if (herSickness >= maxHerSickness)
            {
                if (onMatchLostBefore != null)
                    onMatchLostBefore.Invoke();

                Invoke("MatchEndDelayed", delayAfterDead);
                Time.timeScale = 0.5f;
            }
        }
    }
    void MatchEndDelayed()
    {
        //setup best
        bool newRecord = false;
        if (GetPoints() > GetBestScore())
        {
            SetBestScore(GetPoints());
            newRecord = true;
        }

        herSickness = 0;
        StopAndThrowLostEvent(true, newRecord);
        wasGameOver = true;

        AchievmentsManager.Instance.Unlock<FirstMatchCompleted>();
        Time.timeScale = 1f;
    }
    #endregion
}
