using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun Data/WaveSettingsRapid")]
public class WaveSettingsRapidData : WaveSettingsData
{
    [Header("Behaviour")]
    public float frequencyMin;
    public float frequencyMax;

    public int objevctPerTickMin;
    public int objectPerTickMax;

    public int objectsPerTickStepMin;
    public int objectsPerTickStepMax;

    public int tickCount;
}
