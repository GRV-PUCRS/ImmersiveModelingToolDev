using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CustomUnityEvents;

public class ServerNetwork : MonoBehaviourPunCallbacks
{
    public static ServerNetwork Instance { get; private set; }

    public UnityEventByteArray OnReceiveByteArray;
    public UnityEventPositionMessage OnReceiveUpdateCamera;
    public UnityEventPositionMessage OnReceiveUpdateTag;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Conectado no Master");

        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        UIController.Instance.SetPlayerCount(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        UIController.Instance.SetRoom(PhotonNetwork.CurrentRoom.Name);
        UIController.Instance.SetPlayerCount(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private void OnEventReceived(EventData message)
    {
        Debug.Log("Network recebeu mensagem");

        switch (message.Code)
        {
            case Events.UPDATE_FRAME:
                OnReceiveByteArray?.Invoke((byte[])message.CustomData);

                break;

            case Events.UPDATE_CAMERA:
                OnReceiveUpdateCamera?.Invoke((object[])message.CustomData);

                break;

            case Events.UPDATE_TAG:
                OnReceiveUpdateTag?.Invoke((object[])message.CustomData);

                break;

            default:
                Debug.Log($"Evento {message.Code} nao tratado!");

                break;
        }
    }

    public void OnSendMessageRequest(byte code, object[] message)
    {
        PhotonNetwork.RaiseEvent(code, message, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnSendMessageRequest += OnSendMessageRequest;
    }

    public override void OnDisable()
    {
        EventManager.OnSendMessageRequest += OnSendMessageRequest;
        base.OnDisable();
    }
}