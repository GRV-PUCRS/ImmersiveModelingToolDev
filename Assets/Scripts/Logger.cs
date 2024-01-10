
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace GRV
{
    // Easy and quick way to see logs in mobile application
    // Author: Vinicius Chrisosthemos Teixeira
    // Date: 01/03/2023
    // Based on: https://answers.unity.com/questions/125049/is-there-any-way-to-view-the-console-in-a-build.html
    public class Logger : Singleton<Logger>
    {
        [Header("Settings")]
        [SerializeField] private int maxPrint = 40;

        // control variables
        static string myLog = "";
        private string output;
        private int count = 0;
        private string logFilePath = "";

        private void Awake()
        {
            logFilePath = Application.persistentDataPath + "/log.txt";
        }

        // Bind method to every message received
        void OnEnable()
        {
            Application.logMessageReceived += Log;

            if (File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "");
            }
            else
            {
                File.Create(logFilePath);
            }
        }

        // Unbind method
        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        // Save the message and concatenate with the previous messages
        public void Log(string logString, string stackTrace, LogType type)
        {
            output = $"[{type}][{DateTime.Now.ToString("HH:mm:ss:fff")}]{logString}";

            if (type == LogType.Exception)
            {
                output += $"\nStackTracen{stackTrace}\n";
            }

            // Clear if max amount of message are reached
            if (count >= maxPrint)
            {
                SaveLog(myLog);

                myLog = output;
                count = 0;
            }
            else
            {
                myLog += Environment.NewLine + output;
            }

            count++;
        }

        private void SaveLog(string newContent)
        {
            File.AppendAllText(logFilePath, newContent + Environment.NewLine);
        }

        private void OnDestroy()
        {
            SaveLog(myLog);
        }
    }
}