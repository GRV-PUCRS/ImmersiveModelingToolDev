using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using CustomUnityEvents;
using System.IO;
using System.Collections;
using static Events;
using System;

public class NetworkPhoton : MonoBehaviourPunCallbacks
{
    public static NetworkPhoton Instance { get; private set; }

    [Header("Configuracao do Server")]
    [SerializeField] private byte maxClients = 16;

    [Header("Callbacks dos Eventos do Sistema")]
    public UnityEvent<object[]> OnFileTransferRequest;
    public UnityEvent<object[]> OnFileTransferUpload;
    public UnityEvent<object[]> OnFileTransferInfo;
    public UnityEvent<object[]> OnFileTransferFile;
    public UnityEvent OnFileTransferConfirmation;
    public UnityEvent<object[]> OnFileSearchRequest;
    public UnityEvent<object[]> OnFileSearchResponse;
    public UnityEvent<object[]> OnObjectTrackerMessage;

    [Header("Callbacks dos Eventos do Photon")]
    public UnityEvent OnPlayerConnectedToMasterEvent;
    public UnityEvent<string, int> OnPlayerLeftRoomEvent;
    public UnityEvent OnCreateRoomEvent;
    public UnityEvent<bool> OnCloseConnectionEvent;
    public UnityEventInt OnClientEnteredRoomEvent;
    public UnityEventInt OnJoinedRoomEvent;
    public UnityEventString OnPlayerEnteredRoomEvent;
    public UnityEventString OnCreateRoomFailedEvent;
    public UnityEventString OnJoinRoomFailedEvent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Start");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        //UnityEngine.XR.XRSettings.enabled = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao Master");
        base.OnConnectedToMaster();

        OnPlayerConnectedToMasterEvent?.Invoke();
    }

    public void CloseConnection()
    {
        PhotonNetwork.LeaveRoom();

        OnCloseConnectionEvent?.Invoke(PhotonNetwork.IsMasterClient);
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("Entrando na Room: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreateRoom(string roomName)
    {
        Debug.Log("Criando Room: " + roomName);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxClients }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Criado!");
        base.OnCreatedRoom();
        OnCreateRoomEvent?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("On Create Room Failed: " + returnCode + "  "+ message);

        base.OnCreateRoomFailed(returnCode, message);

        OnCreateRoomFailedEvent?.Invoke(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("On Player Entered Room: " + newPlayer.NickName);

        base.OnPlayerEnteredRoom(newPlayer);

        OnPlayerEnteredRoomEvent?.Invoke(newPlayer.NickName);
        OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player saiu da Room: " + otherPlayer.NickName);
        base.OnPlayerLeftRoom(otherPlayer);

        OnPlayerLeftRoomEvent?.Invoke(otherPlayer.NickName, PhotonNetwork.CountOfPlayers);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On Joined Room!");
        base.OnJoinedRoom();

        OnJoinedRoomEvent?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("On Join Room Failed");

        base.OnJoinRoomFailed(returnCode, message);

        OnJoinRoomFailedEvent?.Invoke(message);
    }

    private void OnEventReceived(EventData message)
    {
        switch (message.Code)
        {
            case FILE_TRANSFER_EVENT:
                //Debug.Log("[NetworkPhoton] Evento FILE TRANSFER recebido!");

                HandleFileTransferEvent((object[])message.CustomData);
                break;

            case FILE_SEARCH_EVENT:
                //Debug.Log("[NetworkPhoton] Evento FILE SEARCH recebido!");

                HandleFileSearchEvent((object[])message.CustomData);
                break;

            case OBJECT_TRACKER_EVENT:
                HandleObjectTrackerEvent((object[])message.CustomData);
                break;

            default:
                //Debug.Log($"Evento {message.Code} nao tratado!");

                break;
        }
    }

    private void HandleObjectTrackerEvent(object[] message)
    {
        OnObjectTrackerMessage?.Invoke(message);
    }

    private void HandleFileSearchEvent(object[] message)
    {
        FILE_SEARCH_EVENT_CODE code = (FILE_SEARCH_EVENT_CODE)message[0];

        switch (code)
        {
            case FILE_SEARCH_EVENT_CODE.REQUEST:
                OnFileSearchRequest?.Invoke(message);

                break;

            case FILE_SEARCH_EVENT_CODE.RESPONSE:
                OnFileSearchResponse?.Invoke(message);

                break;
        }
    }

    private void HandleFileTransferEvent(object[] message)
    {
        FILE_TRANSFER_EVENT_CODE code = (FILE_TRANSFER_EVENT_CODE)message[0];
        //Debug.Log("=> " + code.ToString());
        switch (code)
        {
            case FILE_TRANSFER_EVENT_CODE.REQUEST:
                OnFileTransferRequest?.Invoke(message);

                break;

            case FILE_TRANSFER_EVENT_CODE.UPLOAD:
                OnFileTransferUpload?.Invoke(message);

                break;

            case FILE_TRANSFER_EVENT_CODE.INFO:
                OnFileTransferInfo?.Invoke(message);

                break;

            case FILE_TRANSFER_EVENT_CODE.FILE:
                OnFileTransferFile?.Invoke(message);

                break;

            case FILE_TRANSFER_EVENT_CODE.CONFIRMATION:
                OnFileTransferConfirmation?.Invoke();

                break;

        }
    }

    public void OnSendMessageRequest(byte code, object[] message)
    {
        PhotonNetwork.RaiseEvent(code, message, new RaiseEventOptions { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others }, SendOptions.SendUnreliable);
    }

    public void OnSendMessageToServerRequest(byte code, object[] message)
    {
        PhotonNetwork.RaiseEvent(code, message, new RaiseEventOptions { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.MasterClient }, SendOptions.SendUnreliable);
    }

    public override void OnEnable()
    {
        EventManager.OnSendMessageRequest += OnSendMessageRequest;
        EventManager.OnSendMessageToServerRequest += OnSendMessageToServerRequest;
        base.OnEnable();
    }

    public override void OnDisable()
    {
        EventManager.OnSendMessageRequest -= OnSendMessageRequest;
        EventManager.OnSendMessageToServerRequest -= OnSendMessageToServerRequest;
        base.OnDisable();
    }

    public void SendFrame(byte[] message)
    {
        PhotonNetwork.RaiseEvent(Events.UPDATE_FRAME, message, new RaiseEventOptions { CachingOption = EventCaching.DoNotCache }, SendOptions.SendUnreliable);
    }

    public bool InRoom { get => PhotonNetwork.InRoom; }
    public string RoomID { get => PhotonNetwork.CurrentRoom.Name; }
    public bool IsConnected { get => PhotonNetwork.IsConnectedAndReady; }
    public bool IsMasterClient { get => PhotonNetwork.IsMasterClient; }
}