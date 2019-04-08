using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MLog
{
    public static void Info(string log)
    {
#if UNITY_EDITOR || M_DEBUG
        Debug.Log(log);
#endif
    }
    public static void Error(string log)
    {
#if UNITY_EDITOR || M_DEBUG
        Debug.LogError(log);
#endif
    }
    public static void Warning(string log)
    {
#if UNITY_EDITOR || M_DEBUG
        Debug.LogWarning(log);
#endif
    }
}
