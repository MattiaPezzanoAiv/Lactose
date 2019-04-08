using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Animation settingsRoot;
    public UnityEngine.UI.Text tfVersion;

    private void Awake()
    {
        Instance = this;

        showingSettings = false;
        settingsRoot.clip.legacy = true;
        settingsRoot.clip.wrapMode = WrapMode.Once;
        settingsRoot.clip.SampleAnimation(settingsRoot.gameObject,0f);

        soundEnabled = PlayerPrefs.GetInt("SoundsEnabled", 0) == 0 ? true : false;

        tfVersion.text = GameManager.GAME_VERSION;
    }

    private bool showingSettings;

    private bool soundEnabled = true;

    public void ToggleSettings()
    {
        showingSettings = !showingSettings;
        if (showingSettings)
            PlayShow();
        else
            PlayHide();
    }
    void PlayShow()
    {
        settingsRoot[settingsRoot.clip.name].speed = 1f;
        settingsRoot[settingsRoot.clip.name].normalizedTime = 0f;

        settingsRoot.Play();
    }
    void PlayHide()
    {
        settingsRoot[settingsRoot.clip.name].speed = -1f;
        settingsRoot[settingsRoot.clip.name].normalizedTime = 1f;

        settingsRoot.Play();
    }



    public int GetSavedTrail()
    {
        return PlayerPrefs.GetInt("PlayerTrail", 0);
    }
    public void SaveTrail(int idx)
    {
        PlayerPrefs.SetInt("PlayerTrail", idx);
    }
    public bool IsSettingEnabled(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 0 ? true : false;
    }
    public bool IsSoundEnabled()
    {
        return soundEnabled;
    }
    public void ToggleSounds()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("SoundsEnabled", soundEnabled ? 0 : 1);
    }
}
