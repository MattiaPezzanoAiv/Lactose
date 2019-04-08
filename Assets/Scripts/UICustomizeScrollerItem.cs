using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICustomizeScrollerItem : MonoBehaviour
{
    public RectTransform root;
    public Text tfName, tfLockDescr;
    public Image selectionStar;
    public Image bg;
    public Image locked;
    public Button btn;
    public Animation unlockAnim;

    private TrailData myData;
    private Coroutine animCoroutine;
    int myIndex;

    private void Awake()
    {
        this.unlockAnim.clip.legacy = true;
        this.unlockAnim.clip.wrapMode = WrapMode.Once;
    }

    public void Setup(bool selected, int idx, TrailData data, UICustomizePage page, Color lockBgColor)
    {
        myData = data;

        myIndex = idx;
        tfName.text = data.name;
        tfLockDescr.text = data.lockedLabel;

        var color = myData.trail.GetComponent<ParticleSystem>().main.startColor.color;
        color.a = 0.4f;
        bg.color = color;
        locked.color = lockBgColor;

        if (selected)
            Select();
        else
            Deselect();

        locked.gameObject.SetActive(!myData.IsScoreEnough() || !myData.IsOkForAchiev());

        //save callback
        btn.onClick.AddListener(() => page.SelectCurrent(idx));
    }

    public bool IsGoingToBeUnlocked()
    {
        return myData.IsScoreEnough() && myData.IsOkForAchiev() && this.locked.gameObject.activeSelf;
    }
    private void OnEnable()
    {
        if (myData == null) return;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = null;

        if (IsGoingToBeUnlocked())
        {
            Invoke("UnlockAnim", 0.4f);
        }
    }
    
    void UnlockAnim()
    {
        this.unlockAnim.Rewind();
        this.unlockAnim.Play();
        Invoke("DisableLock", this.unlockAnim.clip.length);
    }
    void DisableLock()
    {
        this.locked.gameObject.SetActive(false);
    }
    public void Select()
    {
        selectionStar.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
        PlayerInput.Instance.SetupTrail(myIndex, true);
    }
    public void Deselect()
    {
        selectionStar.gameObject.SetActive(false);
    }
    public void Vibrate()
    {
        if (animCoroutine != null) return;

        animCoroutine = StartCoroutine(VibrationAnim());
    }
    IEnumerator VibrationAnim()
    {
        Vector3 axis = Vector3.forward;
        float angle = 80;
        int dir = Random.Range(0f, 1f) > .5f ? 1 : -1;
        angle *= dir;

        for (float i = 0, j = 0; i < 0.4f; i += Time.deltaTime, j += Time.deltaTime)
        {
            if(j > 0.05f)
            {
                j = 0;
                angle *= -1;
            }

            root.Rotate(axis, angle * Time.deltaTime);
            yield return null;
        }
        root.rotation = Quaternion.identity;
        animCoroutine = null;
    }
}
