using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchiItem : MonoBehaviour
{
    public RectTransform root;
    public Text tfName, tfLockDescr;
    public Image bg;
    public Image locked;
    public Animation unlockAnim;

    private Coroutine animCoroutine;
    private Achievment myAchi;

    private void Awake()
    {
        this.unlockAnim.clip.legacy = true;
        this.unlockAnim.clip.wrapMode = WrapMode.Once;
    }

    public void Setup(Achievment achi, UIAchiListPage page, Color lockBgColor)
    {
        myAchi = achi;

        tfName.text = achi.name;
        tfLockDescr.text = achi.lockedLabel;

        locked.color = lockBgColor;
        locked.gameObject.SetActive(!IsOkForAchiev());

    }

    private void OnEnable()
    {
        if (myAchi == null) return;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = null;

        if (IsGoingToBeUnlocked())
        {
            Invoke("UnlockAnim", 0.4f);
        }
    }
    public bool IsGoingToBeUnlocked()
    {
        return IsOkForAchiev() && this.locked.gameObject.activeSelf;
    }
    bool IsOkForAchiev()
    {
        return AchievmentsManager.Instance.IsAchievmentUnlocked(myAchi.GetType().Name);
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
            if (j > 0.05f)
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
