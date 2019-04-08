using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchiListPage : MonoBehaviour, UIManager.IBackButtonHandler
{
    public UIAchiItem itemPrefab;
    public ScrollRect objectsRoot;

    public Color startColor, endColor;

    List<UIAchiItem> achiInstances;

    private void OnEnable()
    {
        if (achiInstances == null) return;

        int idx = 0;
        foreach (var achi in achiInstances)
        {
            if (achi.IsGoingToBeUnlocked())
                idx = achiInstances.IndexOf(achi);
        }

        float pos = (float)idx / achiInstances.Count;
        objectsRoot.verticalNormalizedPosition = Mathf.Clamp01(1 - pos);
        Debug.Log("POs set at " + Mathf.Clamp01(1 - pos));
    }

    // Start is called before the first frame update
    void Start()
    {
        achiInstances = new List<UIAchiItem>();

        //var trails = PlayerInput.Instance.GetAvailableTrails(ref selected);
        //var colors = PlayerInput.Instance.GetAvailableTrailsColors();
        //var unlockeds = PlayerInput.Instance.GetUnlockedTrails();

        var achievs = AchievmentsManager.Instance.GetAchievements();

        Color bgColor;
        int i = 0;
        foreach (var achi in achievs)
        {
            var inst = Instantiate(itemPrefab, objectsRoot.content);
            inst.gameObject.name = achi.Value.name;

            float delta = (float)i / achievs.Count;
            bgColor = Color.Lerp(startColor, endColor, delta);

            inst.Setup(achi.Value, this, bgColor);
            achiInstances.Add(inst);

            i++;
        }

        itemPrefab.gameObject.SetActive(false);
        objectsRoot.verticalNormalizedPosition = 0;
    }

    public void OnBackButtonReleased()
    {
        UIManager.Instance.Show("UIMainPage");
    }
}
