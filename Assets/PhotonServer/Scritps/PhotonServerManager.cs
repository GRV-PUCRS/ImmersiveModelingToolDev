using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PhotonServerManager : Singleton<PhotonServerManager>
{
    public enum FileRequestType { Obj = 0, Session };
    public enum RequestType { Search = 0, Acquisition };

    private string serverID;
    private string serverPassword;
    private string objectFolderPath;
    private string sessionFolderPath;

    private bool connectionStatus = false;
    private string serverCredencialsCache = "serverCache.txt";

    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject senderPrefab;
    private List<PhotonFileRequestSender> senders;

    private void Start()
    {
        LoadServerCache();

        senders = new List<PhotonFileRequestSender>();
    }

    public void ChangeConnectionStatus(string roomID)
    {
        if (connectionStatus)
        {
            NetworkPhoton.Instance.CloseConnection();
            UIManagerPhotonServer.Instance.SetPlayerConnected(0);
        }
        else
        {
            if (roomID.Equals(""))
            {
                Logger.Log("[PhotonServerManager] Insira um ID para o servidor!");
                return;
            }

            NetworkPhoton.Instance.CreateRoom(roomID);
        }
    }

    private void OnCreateRoomFailed(string message)
    {
        Logger.Log("Erro ao criar Room: " + message);

        connectionStatus = false;

        UIManagerPhotonServer.Instance.SetServerStatus(false);
    }
    private void OnCreateRoom()
    {
        Logger.Log("Room criada com sucesso!");

        connectionStatus = true;

        UIManagerPhotonServer.Instance.SetServerStatus(true);
    }

    private void OnJoinedRoom(int playerCount)
    {
        Logger.Log("Players conectedos: " + playerCount);

        UIManagerPhotonServer.Instance.SetPlayerConnected(playerCount);
    }

    private void OnCloseConnectionEvent(bool isMasterClient)
    {
        connectionStatus = false;

        UIManagerPhotonServer.Instance.SetServerStatus(false);
    }

    public void SendFile(string fileName, List<string> files)
    {
        string filePath = files.Find(f => f.EndsWith(fileName));

        if (filePath == null)
        {
            Logger.Log($"[PhotonServerManager] Arquivo {fileName} nao encontrado!");
            return;
        }

        Logger.Log($"[PhotonServerManager] Inicia Sender de {fileName}");
        PhotonFileRequestSender sender = Instantiate(senderPrefab, requestParent).GetComponent<PhotonFileRequestSender>();

        senders.Add(sender);

        sender.OnDownloadCompleted += OnSenderFinished;
        sender.SetSenderInfo(fileName, File.ReadAllBytes(filePath));
        sender.Process();
    }

    private void OnSenderFinished(PhotonFileRequestSender instance)
    {
        Logger.Log("[PhotonServerManager] Sender finalizou o envio do arquivo!");

        senders.Remove(instance);
        Destroy(instance.gameObject);
    }

    private void OnFileRequestReceived(object[] message)
    {
        FileRequestType fileType = (FileRequestType)message[1];
        string fileName = (string)message[2];

        Logger.Log("FileRequestReceived   " + fileType.ToString());

        switch (fileType)
        {
            case FileRequestType.Obj:
                SendFile(fileName, Directory.GetFiles(ObjectFolderPath).Where(file => !file.Contains('.')).ToList());

                break;
            case FileRequestType.Session:
                SendFile(fileName, Directory.GetFiles(SessionFolderPath).Where(file => file.EndsWith(".json")).ToList());

                break;
        }
    }

    public void LoadServerCache()
    {
        string path = Application.persistentDataPath + "/" + serverCredencialsCache;

        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            string[] lines = content.Split(';');

            if (lines.Length != 4) return;

            ServerID = lines[0];
            ServerPassword = lines[1];
            ObjectFolderPath = lines[2];
            SessionFolderPath = lines[3];
        }
    }

    public void SaveServerCache()
    {
        string content = UIManagerPhotonServer.Instance.ServerID + ";";
        content += UIManagerPhotonServer.Instance.ServerPassword + ";";
        content += UIManagerPhotonServer.Instance.ObjectFolder + ";";
        content += UIManagerPhotonServer.Instance.SessionFolder;

        File.WriteAllText(Application.persistentDataPath + "/" + serverCredencialsCache, content);
    }

    private void OnPlayerLeftRoom(string playerNickName, int playerCount)
    {
        Logger.Log("Player "+playerNickName+" desconectado!");

        UIManagerPhotonServer.Instance.SetPlayerConnected(playerCount);
    }

    private void SendListOfFiles(List<string> fileNames)
    {
        Logger.Log($"[PhotonServerManager] Envia {fileNames.Count} arquivos!");

        for (int i = 0; i < fileNames.Count; i++)
        {
            fileNames[i] = Utils.GetFileNameFromPath(fileNames[i]);
        }

        EventManager.TriggerSendMessageRequest(Events.FILE_SEARCH_EVENT, new object[2] { Events.FILE_SEARCH_EVENT_CODE.RESPONSE, fileNames.ToArray() });
    }

    private void OnFileSearchRequest(object[] message)
    {
        FileRequestType requestType = (FileRequestType)message[1];

        switch (requestType)
        {
            case FileRequestType.Obj:
                SendListOfFiles(Directory.GetFiles(ObjectFolderPath).Where(file => !file.Contains('.')).ToList());

                break;
            case FileRequestType.Session:
                SendListOfFiles(Directory.GetFiles(SessionFolderPath).Where(file => file.EndsWith(".json")).ToList());

                break;
        }
    }

    private void OnEnable()
    {
        if (NetworkPhoton.Instance == null)
        {
            Logger.Log("NetworkPhoton nao encontrado!");
        }
        else
        {
            NetworkPhoton.Instance.OnCreateRoomFailedEvent.AddListener(OnCreateRoomFailed);
            NetworkPhoton.Instance.OnJoinedRoomEvent.AddListener(OnJoinedRoom);
            NetworkPhoton.Instance.OnCreateRoomEvent.AddListener(OnCreateRoom);
            NetworkPhoton.Instance.OnCloseConnectionEvent.AddListener(OnCloseConnectionEvent);
            NetworkPhoton.Instance.OnPlayerLeftRoomEvent.AddListener(OnPlayerLeftRoom);

            NetworkPhoton.Instance.OnFileTransferRequest.AddListener(OnFileRequestReceived);
            NetworkPhoton.Instance.OnFileSearchRequest.AddListener(OnFileSearchRequest);
            //NetworkPhoton.Instance.OnFileRequestReceived.AddListener
        }
    }

    private void OnDisable()
    {
        if (NetworkPhoton.Instance == null)
        {
            Logger.Log("NetworkPhoton nao encontrado!");
        }
        else
        {
            NetworkPhoton.Instance.OnCreateRoomFailedEvent.RemoveListener(OnCreateRoomFailed);
            NetworkPhoton.Instance.OnJoinedRoomEvent.RemoveListener(OnJoinedRoom);
            NetworkPhoton.Instance.OnCreateRoomEvent.RemoveListener(OnCreateRoom);
            NetworkPhoton.Instance.OnCloseConnectionEvent.RemoveListener(OnCloseConnectionEvent);
            NetworkPhoton.Instance.OnPlayerLeftRoomEvent.RemoveListener(OnPlayerLeftRoom);


            NetworkPhoton.Instance.OnFileTransferRequest.RemoveListener(OnFileRequestReceived);
            NetworkPhoton.Instance.OnFileSearchRequest.RemoveListener(OnFileSearchRequest);
        }
    }

    public string ServerID { get => serverID; set
        {
            serverID = value;
            UIManagerPhotonServer.Instance.ServerID = serverID;
        } }
    public string ServerPassword { get => serverPassword; set
        {
            serverPassword = value;
            UIManagerPhotonServer.Instance.ServerPassword = serverPassword;
        } }
    public string ObjectFolderPath { get => objectFolderPath; set
        {
            objectFolderPath = value;
            UIManagerPhotonServer.Instance.ObjectFolder = objectFolderPath;
        } }
    public string SessionFolderPath { get => sessionFolderPath; set
        {
            sessionFolderPath = value;
            UIManagerPhotonServer.Instance.SessionFolder = sessionFolderPath;
        } }
}
