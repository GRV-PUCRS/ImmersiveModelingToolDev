using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PhotonServerManager : Singleton<PhotonServerManager>
{
    public enum FileRequestType { Obj = 0, Session };
    public enum RequestType { Search = 0, Acquisition };

    private const string DEFAULT_SESSION_FOLDER = "sessions";
    private const string DEFAULT_OBJECTS_FOLDER = "objects";
    private const string DEFAULT_SERVER_CACHE_FILE_NAME = "serverCache.txt";

    private string serverID;
    private string serverPassword;
    private string objectFolderPath;
    private string sessionFolderPath;

    private bool connectionStatus = false;
    private string serverCachePath;


    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject senderPrefab;
    [SerializeField] private GameObject receiverPrefab;
    private List<PhotonFileRequestSender> senders;
    private List<PhotonFileRequestReceiver> receivers;

    private void Start()
    {
        serverCachePath = Path.Combine(Application.persistentDataPath, DEFAULT_SERVER_CACHE_FILE_NAME);

        LoadServerCache();

        senders = new List<PhotonFileRequestSender>();
        receivers = new List<PhotonFileRequestReceiver>();
    }

    public void ChangeConnectionStatus(string roomID)
    {
        if (connectionStatus)
        {
            NetworkPhoton.Instance.CloseConnection();
            UIManagerPhotonServer.Instance.SetPlayerConnected(0);
            UIManagerPhotonServer.Instance.SetInputAvailable(true);
        }
        else
        {
            if (roomID.Equals(""))
            {
                Debug.Log("[PhotonServerManager] Insira um ID para o servidor!");
                return;
            }

            NetworkPhoton.Instance.CreateRoom(roomID);
            SaveServerCache();
            UIManagerPhotonServer.Instance.SetInputAvailable(false);
        }
    }

    private void OnCreateRoomFailed(string message)
    {
        Debug.Log("Erro ao criar Room: " + message);

        connectionStatus = false;

        UIManagerPhotonServer.Instance.SetServerStatus(false);
    }
    private void OnCreateRoom()
    {
        Debug.Log("Room criada com sucesso!");

        connectionStatus = true;

        UIManagerPhotonServer.Instance.SetServerStatus(true);
    }

    private void OnJoinedRoom(int playerCount)
    {
        Debug.Log("Players conectedos: " + playerCount);

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
            Debug.Log($"[PhotonServerManager] Arquivo {fileName} nao encontrado!");
            return;
        }

        Debug.Log($"[PhotonServerManager] Inicia Sender de {fileName}");
        PhotonFileRequestSender sender = Instantiate(senderPrefab, requestParent).GetComponent<PhotonFileRequestSender>();

        senders.Add(sender);

        sender.OnDownloadCompleted += OnSenderFinished;
        sender.SetSenderInfo(fileName, File.ReadAllBytes(filePath));
        sender.Process();
    }

    private void OnSenderFinished(PhotonFileRequestSender instance)
    {
        Debug.Log("[PhotonServerManager] Sender finalizou o envio do arquivo!");

        senders.Remove(instance);
        Destroy(instance.gameObject);
    }

    private void OnFileRequestReceived(object[] message)
    {
        FileRequestType fileType = (FileRequestType)message[1];
        string fileName = (string)message[2];

        Debug.Log("FileRequestReceived   " + fileType.ToString());

        switch (fileType)
        {
            case FileRequestType.Obj:
                SendFile(fileName, Directory.GetFiles(ObjectFolderPath).ToList());

                break;
            case FileRequestType.Session:
                SendFile(fileName, Directory.GetFiles(SessionFolderPath).Where(file => file.EndsWith(".json")).ToList());

                break;
        }
    }

    public void LoadServerCache()
    {
        if (File.Exists(serverCachePath))
        {
            ServerCacheData cache = JsonUtility.FromJson<ServerCacheData>(File.ReadAllText(serverCachePath));

            ServerID = cache.ServerID;
            ServerPassword = cache.ServerPassword;
            ObjectFolderPath = cache.ObjectsFolderPath;
            SessionFolderPath = cache.SessionsFolderPath;

        }
        else
        {
            string sessionsPath = Path.Combine(Application.persistentDataPath, DEFAULT_SESSION_FOLDER);
            string objectsPath = Path.Combine(Application.persistentDataPath, DEFAULT_OBJECTS_FOLDER);

            if (!Directory.Exists(sessionsPath))
                Directory.CreateDirectory(sessionsPath);

            if (!Directory.Exists(objectsPath))
                Directory.CreateDirectory(objectsPath);

            ServerID = "";
            ServerPassword = "";
            ObjectFolderPath = objectsPath;
            SessionFolderPath = sessionsPath;
        }

        UIManagerPhotonServer.Instance.ServerID = ServerID;
        UIManagerPhotonServer.Instance.ServerPassword = ServerPassword;
        UIManagerPhotonServer.Instance.SessionFolder = SessionFolderPath;
        UIManagerPhotonServer.Instance.ObjectFolder = ObjectFolderPath;
    }

    public void SaveServerCache()
    {
        ServerCacheData cache = new ServerCacheData();

        cache.ServerID = ServerID;
        cache.ServerPassword = ServerPassword;
        cache.ObjectsFolderPath = ObjectFolderPath;
        cache.SessionsFolderPath = SessionFolderPath;

        Debug.Log(cache.ServerID);

        File.WriteAllText(serverCachePath, JsonUtility.ToJson(cache));
    }

    private void OnPlayerLeftRoom(string playerNickName, int playerCount)
    {
        Debug.Log("Player "+playerNickName+" desconectado!");

        UIManagerPhotonServer.Instance.SetPlayerConnected(playerCount);
    }

    private void SendListOfFiles(List<string> fileNames)
    {
        Debug.Log($"[PhotonServerManager] Envia {fileNames.Count} arquivos!");

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
                SendListOfFiles(Directory.GetFiles(ObjectFolderPath).ToList());

                break;
            case FileRequestType.Session:
                SendListOfFiles(Directory.GetFiles(SessionFolderPath).Where(file => file.EndsWith(".json")).ToList());

                break;
        }
    }

    private void OnFileTransferUpload(object[] message)
    {
        Debug.Log("[PhotonServerManager] Recebe evento de Upload");

        FileRequestType requestType = (FileRequestType)message[1];

        switch (requestType)
        {
            case FileRequestType.Obj:
                Debug.Log("[PhotonServerManager] Operacao nao implementada!");

                break;
            case FileRequestType.Session:
                string filePath = (string)message[2];

                ReceiveFileFromUser(requestType, filePath);

                break;
        }
    }

    private void ReceiveFileFromUser(FileRequestType fileType, string filePath)
    {
        PhotonFileRequestReceiver receiver = Instantiate(receiverPrefab, transform).GetComponent<PhotonFileRequestReceiver>();

        receiver.SetReceiverInfo(filePath, fileType, null);

        receivers.Add(receiver);

        StartReceiver();
    }

    public void StartReceiver()
    {
        if (receivers.Count == 0) return;

        receivers[0].OnDownloadCompleted += OnDownloadCompleted;
        receivers[0].Process();
    }

    private void OnDownloadCompleted(PhotonFileRequestReceiver instance)
    {
        Debug.Log($"[PhotonServerManager] Download do arquivo {instance.FileName} concluido.\n  Salvando ...");
        string fileContent = System.Text.Encoding.UTF8.GetString(instance.bufferedFile.data);
        string filePath = Path.Combine(sessionFolderPath, Utils.GetFileNameFromPath(instance.FileName));

        File.WriteAllText(filePath, fileContent);

        receivers.Remove(instance);
        Destroy(instance.gameObject);

        if (receivers.Count == 0)
        {
            Debug.Log("[PhotonServerManager] Nao ha mais arquivos a serem recebidos!");
        }
        else
        {
            StartReceiver();
        }

        Debug.Log($"[PhotonServerManager]   Arquivo salvo com sucesso!");
    }


    private void OnEnable()
    {
        if (NetworkPhoton.Instance == null)
        {
            Debug.Log("NetworkPhoton nao encontrado!");
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
            NetworkPhoton.Instance.OnFileTransferUpload.AddListener(OnFileTransferUpload);

            //NetworkPhoton.Instance.OnFileRequestReceived.AddListener
        }
    }

    private void OnDisable()
    {
        if (NetworkPhoton.Instance == null)
        {
            Debug.Log("NetworkPhoton nao encontrado!");
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
            NetworkPhoton.Instance.OnFileTransferUpload.RemoveListener(OnFileTransferUpload);
        }
    }

    public string ServerID { get => serverID; set
        {
            serverID = value;
        } }
    public string ServerPassword { get => serverPassword; set
        {
            serverPassword = value;
        } }
    public string ObjectFolderPath { get => objectFolderPath; set
        {
            objectFolderPath = value;
        } }
    public string SessionFolderPath { get => sessionFolderPath; set
        {
            sessionFolderPath = value;
        } }
}
