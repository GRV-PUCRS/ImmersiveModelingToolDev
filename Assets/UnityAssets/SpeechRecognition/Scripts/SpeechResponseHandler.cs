using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(VoskSpeechToText), typeof(VoiceProcessor))]
public class SpeechResponseHandler : MonoBehaviour
{
    [SerializeField] private List<string> keyWords;
    private VoskSpeechToText speechRecognizer;

    public UnityEvent<string> OnWordFound;

    private void Awake()
    {
        speechRecognizer = GetComponent<VoskSpeechToText>();
        speechRecognizer.OnTranscriptionResult += OnResponseReceived;
    }

    public void OnResponseReceived(string text)
    {
        SpeechResponse response = JsonUtility.FromJson<SpeechResponse>(text);

        if (response == null) return;

        foreach (var textResponse in response.alternatives)
        {
            string formattedText = textResponse.text.Trim();
            if (keyWords.Contains(formattedText)) OnWordFound?.Invoke(formattedText);
        }
    }

    [Serializable]
    private class TextResponse
    {
        public float confidence;
        public string text;
    }

    [Serializable]
    private class SpeechResponse
    {
        public TextResponse[] alternatives;
    }
}
