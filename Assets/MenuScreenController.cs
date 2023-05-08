using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuScreenController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private FMNetworkManager networkManager;

    [SerializeField] private GameObject menuScreen;

    public void StartStreaming()
    {
        networkManager.ClientSettings.ServerIP = inputField.text;
        networkManager.Action_InitAsClient();

        menuScreen.SetActive(false);
    }

    public void Back()
    {
        menuScreen.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
