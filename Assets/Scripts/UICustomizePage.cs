using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICustomizePage : MonoBehaviour, UIManager.IBackButtonHandler
{
    public UICustomizeScrollerItem itemPrefab;
    public ScrollRect objectsRoot;

    public Color startColor, endColor;

    int selected = 0;
    List<UICustomizeScrollerItem> trailsInstances;

    // Start is called before the first frame update
    void Start()
    {
        trailsInstances = new List<UICustomizeScrollerItem>();

        //var trails = PlayerInput.Instance.GetAvailableTrails(ref selected);
        //var colors = PlayerInput.Instance.GetAvailableTrailsColors();
        //var unlockeds = PlayerInput.Instance.GetUnlockedTrails();

        var trailData = PlayerInput.Instance.GetTrailData();
        selected = SettingsManager.Instance.GetSavedTrail();

        Color c;
        int i = 0;
        foreach (var t in trailData)
        {
            var inst = Instantiate(itemPrefab, objectsRoot.content);
            inst.gameObject.name = t.name;

            float delta = (float)i / trailData.Count;
            c = Color.Lerp(startColor, endColor, delta);

            inst.Setup(i == selected, i, t, this, c);
            trailsInstances.Add(inst);

            i++;
        }

        itemPrefab.gameObject.SetActive(false);
        objectsRoot.verticalNormalizedPosition = 0;
    }
    private void OnEnable()
    {
        if(trailsInstances != null)
        {
            int idx = 0;
            foreach (var trail in trailsInstances)
            {
                if (trail.IsGoingToBeUnlocked())
                    idx = trailsInstances.IndexOf(trail);
            }

            float pos = (float)idx / trailsInstances.Count;
            objectsRoot.verticalNormalizedPosition = Mathf.Clamp01(1 - pos);
        }

        foreach (var t in PlayerInput.Instance.GetTrailData())
            if (!t.IsScoreEnough() || !t.IsOkForAchiev())
                return; //if 1 trail is not available return 

        AchievmentsManager.Instance.Unlock<AllTrailsUnlocked>();
    }
    public void SelectCurrent(int idx)
    {
        DeselectCurrent();

        selected = idx;
        trailsInstances[selected].Select();

        if(idx != 0)
        {
            AchievmentsManager.Instance.Unlock<TrailEquipped>();
        }
    }
    private void DeselectCurrent()
    {
        trailsInstances[selected].Deselect();
    }

    public void OnBackButtonReleased()
    {
        UIManager.Instance.Show("UIMainPage");
    }
}
