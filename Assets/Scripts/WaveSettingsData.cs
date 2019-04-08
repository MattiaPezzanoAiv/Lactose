using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum WaveStartPosition { Right, Bottom }
public enum WaveLevel { Easy, Medium, Hard }
public enum WaveForceMode { Alternate, Random }
public class WaveSettingsData : ScriptableObject
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WaveSettingsData), true)]
    [CanEditMultipleObjects]
    class WaveDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            if (GUILayout.Button("Play"))
            {
                var c = GameObject.FindObjectOfType<ObjectSpawner>();
                if (c != null)
                {
                    if (target as WaveSettingsPreciseData != null)
                        c.MoveNextWave<WaveSettingsPrecise>(target as WaveSettingsData);
                    else
                        c.MoveNextWave<WaveSettingsRapid>(target as WaveSettingsData);
                }
            }
        }
    }
#endif


    [Header("Delay")]
    public float delayBeforeStart;
    public float delayAfterEnd;

    [Header("General")]
    public WaveLevel level;
    public WaveStartPosition startPos;

    [Header("Throw")]
    public Vector2 directionMin;
    public Vector2 directionMax;

    public float forceMin;
    public float forceMax;

    public WaveForceMode forceMode;

    public Vector2 offsetFromCenterMin;
    public Vector2 offsetFromCenterMax;
}

