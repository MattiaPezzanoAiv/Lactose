using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AchievmentsManager))]
public class AchievmentsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var me = (target as AchievmentsManager);
        if(GUILayout.Button("call mathod"))
        {
            me.GetAchievementsBySocialId();
        }
        if (GUILayout.Button("Reset achis"))
        {
            me.ResetAchis();
        }
        if (GUILayout.Button("Unlock 1"))
        {
            me.Unlock<FirstMatchCompleted>();
        }
        if (GUILayout.Button("Unlock 2"))
        {
            me.Unlock<Record10k>();
        }
        base.OnInspectorGUI();
    }
}
#endif

public class AchievmentsManager : SingletonBehaviour<AchievmentsManager>
{
    private Dictionary<string, Achievment> achievments;

    private void Awake()
    {
        Instance = this;

        #region INITIALIZATION
        achievments = new Dictionary<string, Achievment>();
        achievments.Add(typeof(FirstMatchCompleted).Name, new FirstMatchCompleted());
        achievments.Add(typeof(Record10k).Name, new Record10k());
        achievments.Add(typeof(Record25k).Name, new Record25k());
        achievments.Add(typeof(Record50k).Name, new Record50k());
        achievments.Add(typeof(CreditsWatched).Name, new CreditsWatched());
        achievments.Add(typeof(TrailEquipped).Name, new TrailEquipped());
        achievments.Add(typeof(AllTrailsUnlocked).Name, new AllTrailsUnlocked());
        achievments.Add(typeof(TutorialCompleted).Name, new TutorialCompleted());
        achievments.Add(typeof(Collect100kPoints).Name, new Collect100kPoints());
        achievments.Add(typeof(Collect200kPoints).Name, new Collect200kPoints());
        achievments.Add(typeof(Collect500kPoints).Name, new Collect500kPoints());


        //get from saved achi data
        foreach (var achi in achievments)
            achi.Value.unlocked = PlayerPrefs.GetInt(achi.Key, 0) == 0 ? false : true;
        #endregion
    }

    public Dictionary<string, Achievment> GetAchievements()
    {
        return achievments;
    }
    public Dictionary<string, Achievment> GetAchievementsBySocialId()
    {
        var dic = new Dictionary<string, Achievment>();
        foreach (var achi in achievments)
            dic.Add(achi.Value.socialKey, achi.Value);

        return dic;
    }
    public void ResetAchis()
    {
        //todo
        foreach (var achi in achievments)
        {
            PlayerPrefs.SetInt(achi.Key, 0);
            achi.Value.unlocked = false;
        }
    }

    bool IsAchievmentPresent<T>() where T : Achievment
    {
        return achievments.ContainsKey(typeof(T).Name);
    }
    bool IsAchievmentPresent(System.Type type)
    {
        return achievments.ContainsKey(type.Name);
    }

    public bool IsAchievmentUnlocked(string name)
    {
        foreach (var achi in achievments)
            if (achi.Key == name)
                return achi.Value.unlocked;

        return false;
    }
    public bool IsAchievmentUnlocked<T>() where T : Achievment
    {
        if (!IsAchievmentPresent<T>()) return false;
        return achievments[typeof(T).Name].unlocked;
    }
    public bool IsAchievmentUnlocked(System.Type type)
    {
        if (!IsAchievmentPresent(type)) return false;
        return achievments[type.Name].unlocked;
    }


    public void Unlock<T>(bool showUI = true) where T : Achievment
    {
        if (IsAchievmentUnlocked<T>()) return;

        achievments[typeof(T).Name].unlocked = true;
        PlayerPrefs.SetInt(typeof(T).Name, 1);
        SocialManager.Implementation.UnlockAchievement(achievments[typeof(T).Name].socialKey);

        //play anim, sound or similar
        if (!showUI) return;

        UIManager.Instance.ShowInstant("UIAchievementUnlocked");
        UIManager.Instance.Get<UIAchievmentsPage>("UIAchievementUnlocked").PlayAnim(achievments[typeof(T).Name].message);
    }
    public void Unlock(System.Type type, bool allowNative, bool showUI = true)
    {
        if (IsAchievmentUnlocked(type)) return;

        achievments[type.Name].unlocked = true;
        PlayerPrefs.SetInt(type.Name, 1);
        if (allowNative)
            SocialManager.Implementation.UnlockAchievement(achievments[type.Name].socialKey);

        if (!showUI) return;
        UIManager.Instance.ShowInstant("UIAchievementUnlocked");
        UIManager.Instance.Get<UIAchievmentsPage>("UIAchievementUnlocked").PlayAnim(achievments[type.Name].message);
    }
}
