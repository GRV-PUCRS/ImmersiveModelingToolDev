using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManagerPhotonServer : Singleton<UIManagerPhotonServer>
{
    [Header("Server Config Screen")]
    [SerializeField] private TextMeshProUGUI serverStatus;
    [SerializeField] private TextMeshProUGUI playersConnected;
    [SerializeField] private TextMeshProUGUI btnTurnServer;
    [SerializeField] private TMP_InputField inputServerID;
    [SerializeField] private TMP_InputField inputServerPassword;

    [Header("Input Screen")]
    [SerializeField] private TextMeshProUGUI inputMessage;
    [SerializeField] private TMP_InputField inputField;

    [Header("Files Screen")]
    [SerializeField] private TextMeshProUGUI objectsPath;
    [SerializeField] private TextMeshProUGUI sessionsPath;

    [Header("Screens")]
    [SerializeField] private GameObject serverOffScreen;
    [SerializeField] private GameObject getInputScreen;
    [SerializeField] private GameObject filesConfigScreen;

    private TextMeshProUGUI currentInputText = null;

    private void Start()
    {
        serverOffScreen.SetActive(true);
        getInputScreen.SetActive(false);
        filesConfigScreen.SetActive(false);
    }

    public void SetServerStatus(bool isServerOnline)
    {
        if (isServerOnline)
        {
            serverStatus.text = "Online";
            serverStatus.color = Color.green;

            filesConfigScreen.SetActive(true);
            serverOffScreen.SetActive(false);

            btnTurnServer.text = "Turn Off Server";
        }
        else
        {
            serverStatus.text = "Offline";
            serverStatus.color = Color.red;

            filesConfigScreen.SetActive(false);
            serverOffScreen.SetActive(true);

            btnTurnServer.text = "Turn On Server";
        }
    }

    public void SetPlayerConnected(int playersCount)
    {
        playersConnected.text = playersCount.ToString();
    }

    public void ChangeServerStatus()
    {
        PhotonServerManager.Instance.ChangeConnectionStatus(inputServerID.text + inputServerPassword.text);
    }

    #region GetInputScreen
    public void GetInputFromUser(string newMessage, string newValue)
    {
        getInputScreen.SetActive(true);

        inputMessage.text = newMessage;
        inputField.text = newValue;
    }

    public void GetInputResult(bool confimed)
    {
        if (confimed)
            currentInputText.text = inputField.text;

        getInputScreen.SetActive(false);
    }

    #endregion GetInputScreen

    public void SetInputAvailable(bool newValue)
    {
        inputServerID.interactable = inputServerPassword.interactable = newValue;
    }

    public void ChangeObjectFolder()
    {
        FileBrowserManager.OpenFolderBrowser((string result) =>
        {
            PhotonServerManager.Instance.ObjectFolderPath = result;
            objectsPath.text = result;
            PhotonServerManager.Instance.SaveServerCache();
        });
    }

    public void ChangeSessionsFolder()
    {
        FileBrowserManager.OpenFolderBrowser((string result) =>
        {
            PhotonServerManager.Instance.SessionFolderPath = result;
            sessionsPath.text = result;
            PhotonServerManager.Instance.SaveServerCache();
        });
    }

    #region Properties
    public string ServerID { get => inputServerID.text; set => inputServerID.text = value; }
    public string ServerPassword { get => inputServerPassword.text; set => inputServerPassword.text = value; }
    public string ObjectFolder { get => objectsPath.text; set => objectsPath.text = value; }
    public string SessionFolder { get => sessionsPath.text; set => sessionsPath.text = value; }
    #endregion Properties
}
