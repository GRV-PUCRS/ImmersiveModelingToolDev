using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using static SessionManager;

public class SessionTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionID;
    [SerializeField] private UIButton button;

    [SerializeField] private Image imgCloud;
    [SerializeField] private Image imgLocal;

    public void SetSessionInfo(string sessionTitle, SessionFile.SessionFileState sessionState = SessionFile.SessionFileState.NotSync)
    {
        sessionID.text = sessionTitle.Replace("_", " ").Replace(".json", "");

        if (sessionState != SessionFile.SessionFileState.NotSync)
        {
            imgCloud.gameObject.SetActive(sessionState == SessionFile.SessionFileState.Cloud);
            imgLocal.gameObject.SetActive(sessionState == SessionFile.SessionFileState.Local);
        }

    }

    public UIButton Button { get => button; }
}
