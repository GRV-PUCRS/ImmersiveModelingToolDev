using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpeechResponseHandler))]
public class SpeechCommandsManager : Singleton<SpeechCommandsManager>
{
    private SpeechResponseHandler handler;

    public event Action OnCommandNext;
    public event Action OnCommandBack;

    private void Awake()
    {
        handler = GetComponent<SpeechResponseHandler>();

        handler.OnWordFound.AddListener(HandleWordDetected);
    }

    public void HandleWordDetected(string word)
    {
        switch (word)
        {
            case "next": OnCommandNext?.Invoke(); break;
            case "back": OnCommandBack?.Invoke(); break;
            default: break;
        }
    }
}
