using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    //setup tutorial steps (maybe with class with various params like texts and other things)
    //class called tutorial step that has a coroutine that drives itself finished start the next part, if the last part finish tutorial 
    public static TutorialManager Instance { get; private set; }


    public WaveSettingsPreciseData milkData, soyData;
    public SmashableObject milkPrefab, soyPrefab;
    public Sprite herMilkGfx, herSoyGfx;
    public Animation handCutAnimation;
    public PlayerInput playerInput;

    public float delayBeforeStart = 2f;
    public float delayBeforeStep2 = 2f;
    public float delayAfterThrow = 1.5f;
    public float delayForSoy = 2f;

    private Coroutine tutorialCoroutine;
    private bool step1Completed, step2Completed;

    private void Awake()
    {
        Instance = this;
        handCutAnimation.clip.legacy = true;
        handCutAnimation.clip.wrapMode = WrapMode.Loop;
        handCutAnimation.gameObject.SetActive(false);
    }

    public void StartTutorial()
    {
        step1Completed = false;
        step2Completed = false;

        //UIManager.Instance.HideAll();
        playerInput.Disable();
        UIManager.Instance.Show("UITutorialPage");

        tutorialCoroutine = StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {
        var tutorialPage = UIManager.Instance.Get<UITutorialPage>("UITutorialPage");
        tutorialPage.GfxActive(false);
        GameObject.FindObjectOfType<ParallaxMapping>().Play(true);
        tutorialPage.ShowMessage("She doesn't like LACTOSE!");

        yield return new WaitForSeconds(delayBeforeStart);

        var spawner = GameObject.FindObjectOfType<ObjectSpawner>();
        var milkInstance = Instantiate(milkPrefab);
        milkInstance.onSmash += () => step1Completed = true;

        spawner.SimulateOneForTutorial(milkData, milkInstance);

        yield return new WaitForSeconds(delayAfterThrow);

        //active hand animation and image
        milkInstance.Pause();

        tutorialPage.SetGfx(herMilkGfx);
        tutorialPage.GfxActive(true);

        handCutAnimation.gameObject.SetActive(true);
        handCutAnimation.transform.position = milkInstance.transform.position;
        handCutAnimation.Play();

        ParallaxMapping.Instance.Pause();
        foreach (var c in Character.Actives)
            c.Pause();
        

        //handle cut
        yield return new WaitForSeconds(0.5f);
        playerInput.Enable();

        while(!step1Completed)
            yield return null;

        ParallaxMapping.Instance.Play(false);
        foreach (var c in Character.Actives)
            c.Resume();

        handCutAnimation.gameObject.SetActive(false);
        tutorialPage.GfxActive(false);
        playerInput.Disable();

        //step 1 completed!
        yield return new WaitForSeconds(delayBeforeStep2);
        tutorialPage.ShowMessage("She likes SOY!");

        //throw step 2
        var soyInstance = Instantiate(soyPrefab);

        spawner.SimulateOneForTutorial(soyData, soyInstance);

        yield return new WaitForSeconds(delayAfterThrow);

        ParallaxMapping.Instance.Pause();
        foreach (var c in Character.Actives)
            c.ChangeState(c.cheering);

        soyInstance.Pause();
        tutorialPage.SetGfx(herSoyGfx);
        tutorialPage.GfxActive(true);

        yield return new WaitForSeconds(delayForSoy);

        ParallaxMapping.Instance.Play(false);
        foreach (var c in Character.Actives)
            c.ChangeState(c.walking);

        //activate soy
        soyInstance.Resume();
        soyInstance.RemoveForEndMatch(2f);

        AchievmentsManager.Instance.Unlock<TutorialCompleted>();
        yield return new WaitForSeconds(2f);
        GameObject.FindObjectOfType<ParallaxMapping>().Pause();
        tutorialPage.GfxActive(false);
        playerInput.Enable();
        //UIManager.Instance.HideAll();
        UIManager.Instance.Show("UIMainPage");
    }
}
