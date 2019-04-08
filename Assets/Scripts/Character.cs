using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var me = (target as Character);
        if (GUILayout.Button("Idle"))
        {
            me.ChangeState(me.idle);
        }
        if (GUILayout.Button("walk"))
        {
            me.ChangeState(me.walking);
        }
        if (GUILayout.Button("walk sick"))
        {
            me.ChangeState(me.walkingSick);

        }
        if (GUILayout.Button("walk very sick"))
        {
            me.ChangeState(me.walkingVerySick);

        }
        if (GUILayout.Button("happy"))
        {
            me.ChangeState(me.cheering);

        }
        if (GUILayout.Button("dead"))
        {
            me.ChangeState(me.dead);

        }
        base.OnInspectorGUI();
    }
}
#endif

public class Character : MonoBehaviour
{
    public static List<Character> Actives { get; private set; }

    [SerializeField]
    SpriteRenderer rend;

    public MyAnimation idle, walking, walkingSick, walkingVerySick, cheering, dead;

    [SerializeField]
    int walkingSickValue, walkingVerySickValue;

    MyAnimation currentState;

    private void OnEnable()
    {
        if (Actives == null)
            Actives = new List<Character>();

        Actives.Add(this);
    }
    private void OnDisable()
    {
        Actives.Remove(this);
    }


    private void Start()
    {
        GameManager.Instance.onFirstNewBestScoreInMatch += (x) =>
        {
            ChangeState(cheering);
        };
        GameManager.Instance.onFirstNewBestScoreInMatchEnd += () =>
        {
            ChangeState(GetWalkingState());
        };
        GameManager.Instance.onMatchPause += () =>
        {
            currentState.Pause();
        };
        GameManager.Instance.onMatchResume += () =>
        {
            currentState.Resume();
        };
        GameManager.Instance.onMatchStart += () =>
        {
            gameObject.SetActive(true);
            ChangeState(GetWalkingState());
        };
        GameManager.Instance.onMatchLostBefore += () =>
        {
            ChangeState(dead);
        };
        GameManager.Instance.onHerSicknessChanged += (x,y) =>
        {
            if(GetWalkingState() != currentState)
            {
                ChangeState(GetWalkingState());
            }
        };

        ChangeState(idle);
    }


    MyAnimation GetWalkingState()
    {
        var sickness = GameManager.Instance.HerSickness;
        if (sickness >= walkingVerySickValue)
            return walkingVerySick;
        else if (sickness >= walkingSickValue)
            return walkingSick;
        else
            return walking;

    }
    public void ChangeState(MyAnimation newState)
    {
        if (currentState != null)
            currentState.Stop(this);

        currentState = newState;
        currentState.Play(this, rend);
    }

    public void Pause()
    {
        currentState.Pause();
    }
    public void Resume()
    {
        currentState.Resume();
    }
}
