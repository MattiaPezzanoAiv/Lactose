using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyAnimation
{
    public float interval;
    public bool loop;
    public List<Sprite> frames;

    Coroutine coroutine;
    SpriteRenderer renderer;

    bool paused;

    public void Play(MonoBehaviour container, SpriteRenderer rend)
    {
        paused = false;
        this.renderer = rend;
        Stop(container);
        coroutine = container.StartCoroutine(Run());
    }
    public void Stop(MonoBehaviour container)
    {
        paused = false;
        if (coroutine != null)
        {
            container.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void Pause()
    {
        paused = true;
    }
    public void Resume()
    {
        paused = false;
    }


    IEnumerator Run()
    {
        int i = 0;
        while (true)
        {
            while (paused)
                yield return null;

            renderer.sprite = frames[i];
            yield return new WaitForSecondsRealtime(interval);

            if (!loop && i >= frames.Count - 1) //end of anim stop
            {
                coroutine = null;
                yield break;
            }
            else if (loop && i >= frames.Count - 1) //end of anim, loop
            {
                i = 0;
            }
            else
                i++;    //anim is running
        }
    }
}
