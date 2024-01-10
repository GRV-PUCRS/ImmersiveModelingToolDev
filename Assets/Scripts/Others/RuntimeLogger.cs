using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuntimeLogger : MonoBehaviour
{
    public TextMeshProUGUI txtLog;
    public int maxCounter = 15;

    private int counter = 0;

    private void Awake()
    {
        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (counter >= maxCounter)
        {
            txtLog.text = "";
            counter = 0;
        }


        txtLog.text += $"[{type}]{condition}\n";
        
        if (type == LogType.Exception)
        {
            txtLog.text += $"[StackTracer] {stackTrace}";
        }

        counter++;
    }
}
