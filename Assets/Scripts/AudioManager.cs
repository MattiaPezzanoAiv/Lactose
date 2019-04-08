using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonBehaviour<AudioManager>
{

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("setup"))
            {
                var me = (target as AudioManager);

                List<AudioClip> clips = new List<AudioClip>();
                string[] assetsGuid = AssetDatabase.FindAssets("", new string[] { "Assets/AudioClips" });
                foreach (var guid in assetsGuid)
                    clips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid)));

                me.clips = clips;
            }
            base.OnInspectorGUI();
        }
    }
#endif


    AudioSource source;

    [SerializeField]
    List<AudioClip> clips;

    Dictionary<string, AudioClip> _internalClips;

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();

        _internalClips = new Dictionary<string, AudioClip>();
        foreach (var c in clips)
        {
            _internalClips.Add(c.name, c);
        }
    }

    public void PlaySound(string name)
    {
        if(!_internalClips.ContainsKey(name))
        {
            Debug.LogError("Trying to play a sound that doesn't exist. " + name);
            return;
        }
        if (!SettingsManager.Instance.IsSoundEnabled())
            return;

        //set clip
        source.clip = _internalClips[name];
        source.Play();
    }
    public void PlaySoundDelayed(string name, float delay)
    {
        if (!_internalClips.ContainsKey(name))
        {
            Debug.LogError("Trying to play a sound that doesn't exist. " + name);
            return;
        }
        if (!SettingsManager.Instance.IsSoundEnabled())
            return;

        //set clip
        source.clip = _internalClips[name];
        source.PlayDelayed(delay);
    }

    public void Stop()
    {
        source.Stop();
    }
}
