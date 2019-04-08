using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WaveSettings
{
    public bool oneShot = false;


    public ObjectSpawner spawner;
    protected WaveSettingsData data;
    public IEnumerator activeCoroutine;

    protected Vector2 lastDir;
    protected float lastForce;

    protected Vector2 AlternateDir
    {
        get
        {
            lastDir = lastDir == data.directionMin ? data.directionMax : data.directionMin;
            return lastDir;
        }
    }
    protected Vector2 RandomDir
    {
        get { return Vector2.Lerp(data.directionMin, data.directionMax, Random.Range(0f, 1f)); }
    }
    protected float AlternateForce
    {
        get
        {
            lastForce = Mathf.Approximately(lastForce, data.forceMin) ? data.forceMax : data.forceMin;
            return lastForce;
        }
    }
    protected float RandomForce
    {
        get { return Random.Range(data.forceMin, data.forceMax); }
    }


    protected Vector2 GetThrowDir()
    {
        return data.forceMode == WaveForceMode.Random ? this.RandomDir : AlternateDir;
    }
    protected float GetThrowForce()
    {
        return data.forceMode == WaveForceMode.Random ? this.RandomForce : AlternateForce;
    }
    protected Vector2 GetThrowOffset()
    {
        return Vector2.Lerp(data.offsetFromCenterMin, data.offsetFromCenterMax, Random.Range(0f, 1f));
    }

    public virtual void Init(ObjectSpawner spawner, WaveSettingsData data)
    {
        this.spawner = spawner;
        this.data = data;
    }
    public virtual IEnumerator Wave()
    {
        yield return null;
    }


    public static T Get<T>(ObjectSpawner spawner, WaveSettingsData data) where T : WaveSettings, new()
    {
        T inst = new T();
        inst.Init(spawner, data);
        return inst;
    }
    public static WaveSettings GetRandom(ObjectSpawner spawner, WaveSettingsData data, int precisePercentage = 50)
    {
        int p = Random.Range(0, 101);

        if (p < precisePercentage)
            return Get<WaveSettingsPrecise>(spawner, data);
        return Get<WaveSettingsRapid>(spawner, data);
    }

    protected void Assert<Expected>(System.Type t)
    {
        throw new UnityException("Using type => " + t.Name + " --- Expected type => " + typeof(Expected).Name);
    }
}