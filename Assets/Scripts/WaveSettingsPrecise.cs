using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSettingsPrecise : WaveSettings
{
    private WaveSettingsPreciseData myData;

    public override void Init(ObjectSpawner spawner, WaveSettingsData data)
    {
        base.Init(spawner, data);

        var _data = data as WaveSettingsPreciseData;
        if (_data == null)
            Assert<WaveSettingsPreciseData>(data.GetType());

        this.myData = _data;
    }

    public override IEnumerator Wave()
    {
        if (this.data.delayBeforeStart > 0f)
            yield return new WaitForSeconds(this.data.delayBeforeStart);

        foreach (var lists in myData.datas)
        {
            foreach (var l in lists.waves)
            {
                spawner.SimulateOne(l.spawnPoint, l.Direction2D, l.force);
            }
            yield return new WaitForSeconds(myData.delayBetween);
        }

        spawner.IncreaseWaveCount();    //increase wave count for soy

        if (this.data.delayAfterEnd > 0f)
            yield return new WaitForSeconds(this.data.delayAfterEnd);

        if (!oneShot)
            spawner.MoveNextWave();
    }
}
