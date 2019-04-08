using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveSettingsRapid : WaveSettings
{
    protected float freq;
    protected int objectsPerTick;
    protected int tickCount;
    protected int objectsStep;
    protected WaveStartPosition startPos;

    public override void Init(ObjectSpawner spawner, WaveSettingsData data)
    {
        base.Init(spawner, data);
        var _data = data as WaveSettingsRapidData;
        if (_data == null)
            Assert<WaveSettingsRapidData>(data.GetType());

        freq = Random.Range(_data.frequencyMin, _data.frequencyMax);
        this.objectsPerTick = Random.Range(_data.objevctPerTickMin, _data.objectPerTickMax);
        this.objectsStep = Random.Range(_data.objectsPerTickStepMin, _data.objectsPerTickStepMax);
        this.tickCount = _data.tickCount;
        this.startPos = _data.startPos;
    }
    public override IEnumerator Wave()
    {
#if UNITY_EDITOR
        spawner.DrawOffset(startPos, data.offsetFromCenterMin, data.offsetFromCenterMax);
#endif

        if (this.data.delayBeforeStart > 0f)
            yield return new WaitForSeconds(this.data.delayBeforeStart);

        for (int i = 0; i < tickCount; i++)
        {
            List<float> forces = new List<float>();
            List<Vector2> dirs = new List<Vector2>();
            List<Vector2> offsets = new List<Vector2>();
            for (int j = 0; j < this.objectsPerTick; j++)
            {
                forces.Add(this.GetThrowForce());
                dirs.Add(this.GetThrowDir());
                offsets.Add(this.GetThrowOffset());
            }

            spawner.SimulateOneWave(this.objectsPerTick, dirs, forces, offsets, startPos);
            yield return new WaitForSeconds(freq);

            this.objectsPerTick += this.objectsStep;
        }

        if (this.data.delayAfterEnd > 0f)
            yield return new WaitForSeconds(this.data.delayAfterEnd);

        if (!oneShot)
            spawner.MoveNextWave();
    }
}
