using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CustomUnityEvents;

public class ClientNetwork : MonoBehaviourPunCallbacks
{
    public static ClientNetwork Instance { get; private set; }

    public UnityEventPositionMessage OnReceiveUpdateOrigin;
    public UnityEventPositionMessage OnReceiveUpdateObject;
    public UnityEventPositionMessage OnReceiveCreateObject;
    public UnityEventPositionMessage OnReceiveDeleteObject;
    public UnityEventPositionMessage OnReceiveActionObject;

    private const string DEFAULT_ROOM = "Room";
    private const float DEFAULT_CONNECT_TIMEOUT = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    private void OnEventReceived(EventData message)
    {
        switch (message.Code)
        {
            case Events.UPDATE_ORIGIN:
                OnReceiveUpdateOrigin?.Invoke((object[])message.CustomData);

                break;

            case Events.UPDATE_OBJECT:
                OnReceiveUpdateObject?.Invoke((object[])message.CustomData);

                break;

            case Events.CREATE_OBJECT:
                OnReceiveCreateObject?.Invoke((object[]) message.CustomData);

                break;

            case Events.DELETE_OBJECT:
                OnReceiveDeleteObject?.Invoke((object[]) message.CustomData);

                break;

            case Events.ACTION_OBJECT:
                OnReceiveActionObject?.Invoke((object[]) message.CustomData);
                break;

            default:
                Logger.Log($"Evento {message.Code} nao tratado!");
                break;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Logger.Log("Conectado com Master");
        Logger.Log("Conectando ...");

        StartCoroutine(TryToConnectToRoom());
    }

    private IEnumerator TryToConnectToRoom()
    {
        yield return new WaitForSeconds(DEFAULT_CONNECT_TIMEOUT);

        PhotonNetwork.JoinRoom(DEFAULT_ROOM);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StartCoroutine(TryToConnectToRoom());
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Logger.Log($"Conectado em {PhotonNetwork.CurrentRoom.Name}\nConectados: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Logger.Log("Conectado");
    }

    public void SendPositionMessageToServer(byte code, object[] data)
    {
        PhotonNetwork.RaiseEvent(code, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    public void SendFrameToServer(byte[] data)
    {
        PhotonNetwork.RaiseEvent(Events.UPDATE_FRAME, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
}
