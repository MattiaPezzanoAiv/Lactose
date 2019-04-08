using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogItem : MonoBehaviour
{
    public Image ball;
    public Button btn;
    public Text tfLog;

    string stackTrace;

    public void Setup(string log, string stacktrace, LogType logType)
    {
        ball.color = GetLogColor(logType);
        tfLog.text = log;
        this.stackTrace = stacktrace;
    }

    Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Warning:
                return Color.yellow;
            case LogType.Exception:
            case LogType.Error:
                return Color.red;
            case LogType.Log:
            default:
                return Color.cyan;
        }
    }
}
