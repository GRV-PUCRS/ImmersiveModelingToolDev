using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIOculusClient : MonoBehaviour
{
    [Header("Referencias UI Network Info")]
    [SerializeField] private TextMeshProUGUI serverOnlineText;
    [SerializeField] private TextMeshProUGUI serverRoomName;
    [SerializeField] private TextMeshProUGUI playerCounterText;
    [SerializeField] private TextMeshProUGUI logText;

    private void Awake()
    {
        serverOnlineText.text = "Offline";
    }

    public void OnServerEnterredRoom(string roomName)
    {
        serverRoomName.text = $"Room: {roomName}";
        serverOnlineText.text = "Online";
    }

    public void OnServerConnectedToMaster()
    {
        serverOnlineText.text = "Conectado ao Master";
    }

    public void OnRoomChange(int newPlayerCounter)
    {
        playerCounterText.text = $"Conectados: {newPlayerCounter}";
    }

    public void Log(string message)
    {
        logText.text += $"\n{message}";
    }
}
