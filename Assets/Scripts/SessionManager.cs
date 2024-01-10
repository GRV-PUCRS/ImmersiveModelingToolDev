using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using static PhotonServerManager;

public class SessionManager : Singleton<SessionManager>
{
    [Header("Paremeters")]
    [SerializeField] private string sessionFolder = "Session";
    [SerializeField] private string deaultSessionFile = "Default";
    [SerializeField, Range(1, 12)] private int maxItensVisible = 12;

    [Header("References")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private Transform sessionTemplateParent;
    [SerializeField] private GameObject sessionParent;

    [Header("UI")]
    [SerializeField] private GameObject sessionUI;
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private GameObject sessionInfoUI;
    [SerializeField] private TextMeshProUGUI sessionInfoTitleText;
    [SerializeField] private UIButton backButton;
    [SerializeField] private UIButton nextButton;
    [SerializeField] private TextMeshProUGUI serverName;
    [SerializeField] private UIButton startSessionBTN;
    [SerializeField] private UIButton deleteSessionBTN;
    [SerializeField] private UIButton backupSessionBTN;

    [Header("Prefabs")]
    [SerializeField] private GameObject sessionTemplate;
    [SerializeField] private GameObject newSessionTemplate;

    [Header("Photon Config")]
    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject receiverPrefab;
    [SerializeField] private GameObject senderPrefab;
    private List<PhotonFileRequestReceiver> receivers;
    private List<PhotonFileRequestSender> senders;

    private string sessionFolderPath;
    private SessionFile currentSession;

    private List<SessionFile> files;
    private int currentPage = 0;
    private bool isGuest = false;

    private void Start()
    {
        sessionFolderPath= $"{Application.persistentDataPath}/{sessionFolder}";

        senders = new List<PhotonFileRequestSender>();
        receivers = new List<PhotonFileRequestReceiver>();

        if (!Directory.Exists(sessionFolderPath))
        {
            Directory.CreateDirectory(sessionFolderPath);
            Debug.Log("[SessionManager] Diretorio de sessoes criado com sucesso! " + sessionFolderPath);
        }
        else
        {
            Debug.Log("[SessionManager] Diretorio de sessoes encontrado!" + sessionFolderPath);
        }

        NetworkPhoton.Instance.OnFileTransferRequest.AddListener(OnFileRequestReceived);
    }

    public void LoadSessionScreen(bool guest)
    {
        sessionInfoUI.SetActive(false);

        UpdateSessionList(guest);
    }

    private void UpdateSessionList(bool guest)
    {
        isGuest = guest;
        files = new List<SessionFile>();

        sessionUI.SetActive(false);
        loadingUI.SetActive(true);

        if (!guest)
        {
            DownloadSessions();
        }
        else
        {
            GetLocalFiles();
            ResetUI();
        }
    }

    private void OnFileSearchResponse(object[] message)
    {
        NetworkPhoton.Instance.OnFileSearchResponse.RemoveListener(OnFileSearchResponse);
        
        string[] files = (string[])message[1];

        if (files.Length == 0)
        {
            Debug.Log("Nenhum arquivo recebido do servidor!");
            GetLocalFiles();
            ResetUI();

            return;
        }

        foreach (string file in files)
        {
            PhotonFileRequestReceiver receiver = Instantiate(receiverPrefab, requestParent).GetComponent<PhotonFileRequestReceiver>();
            receiver.SetReceiverInfo(file, FileRequestType.Session, null);
            receivers.Add(receiver);
        }

        StartReceiver(receivers[0]);
    }

    private void OnDownloadCompleted(PhotonFileRequestReceiver instance)
    {
        string fileContent = System.Text.Encoding.UTF8.GetString(instance.bufferedFile.data);

        string filePath = Path.Combine(sessionFolderPath, instance.bufferedFile.name);
        
        if (File.Exists(filePath))
        {
            string duplicatedFileContent = File.ReadAllText(filePath);

            if (fileContent != duplicatedFileContent)
            {
                filePath = filePath.Replace(".json", "_2.json");
                Debug.Log($"[SessionManager] Arquivo {instance.bufferedFile.name} ja existe.");
            }
        }
        
        Debug.Log($"[SessionManager] Salvando arquivo em {filePath}");
        File.WriteAllText(filePath, fileContent);
        Debug.Log("[SessionManager] Arquivo salvo com sucesso!");

        instance.OnDownloadCompleted -= OnDownloadCompleted;
        receivers.Remove(instance);
        Destroy(instance.gameObject);

        if (receivers.Count != 0)
        {
            Debug.Log("[SessionManager] Inicia proximo donwload!");
            StartReceiver(receivers[0]);
        }
        else
        {
            Debug.Log("[SessionManager] Fim do download dos objetos!");

            GetLocalFiles();
            ResetUI();
        }
    }

    private void StartReceiver(PhotonFileRequestReceiver receiver)
    {
        receiver.OnDownloadCompleted += OnDownloadCompleted;
        receiver.Process();
    }

    private void DownloadSessions()
    {
        Debug.Log("[SessionManager] Inicia Download das sessoes ...");
        NetworkPhoton.Instance.OnFileSearchResponse.AddListener(OnFileSearchResponse);

        EventManager.TriggerSendMessageRequest(Events.FILE_SEARCH_EVENT, new object[2] { Events.FILE_SEARCH_EVENT_CODE.REQUEST, FileRequestType.Session });
    }

    private void GetLocalFiles()
    {
        Debug.Log("[SessionManager] Carrega sessoes locais:");
        // Adiciona arquivos locais
        foreach (string filePath in Directory.GetFiles(sessionFolderPath))
        {
            string fileContent = File.ReadAllText(filePath);
            string fileName = Utils.GetFileNameFromPath(filePath);

            files.Add(new SessionFile(fileName, filePath, SessionFile.SessionFileState.Local, fileContent));
            Debug.Log("[SessionManager]    " + fileName);
        }

        Debug.Log("[SessionManager] Arquivos carregados com sucesso!");
    }

    private void ResetUI()
    {
        currentPage = 0;

        sessionUI.SetActive(true);
        loadingUI.SetActive(false);

        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("[SessionManager] Update UI");

        Utils.ClearChilds(sessionTemplateParent);

        int beginIndex = currentPage * maxItensVisible;
        int endIndex = Mathf.Min(files.Count, beginIndex + maxItensVisible - 1);
        SessionTemplate sessionTemplateInstance;

        if (currentPage == 0)
        {
            sessionTemplateInstance = Instantiate(newSessionTemplate, sessionTemplateParent).GetComponent<SessionTemplate>();
            sessionTemplateInstance.SetSessionInfo("New Session");
            sessionTemplateInstance.Button.callbacks.AddListener(() => CreateSession());

            backButton.Interactable = false;
        }
        else
        {
            backButton.Interactable = true;
        }

        for (int i = beginIndex; i < endIndex; i++)
        {
            SessionFile sessionFile = files[i];
            string formatedName = sessionFile.name.Replace(".json", "").Replace("_", " ");

            Debug.Log("[SessionManager] Arquivo de sessao encontrado: " + sessionFile.name);
            sessionTemplateInstance = Instantiate(sessionTemplate, sessionTemplateParent).GetComponent<SessionTemplate>();
            sessionTemplateInstance.SetSessionInfo(formatedName, sessionFile.state);
            sessionTemplateInstance.Button.callbacks.AddListener(() => SelectSession(sessionFile, formatedName));
        }

        nextButton.Interactable = endIndex < files.Count;

        Debug.Log("[SessionManager] Finish Update UI");

        loadingUI.SetActive(false);
    }

    public void NextPage()
    {
        if ((currentPage + 1) * maxItensVisible < files.Count)
        {
            currentPage += 1;
        }

        UpdateUI();
    }

    public void BackPage()
    {
        if (currentPage > 0)
        {
            currentPage -= 1;
        }

        UpdateUI();
    }

    public void SelectSession(SessionFile sessionFile, string formattedName)
    {
        currentSession = sessionFile;

        sessionInfoUI.SetActive(true);
        sessionInfoTitleText.text = formattedName;
    }

    public void LoadSession()
    {
        sessionParent.SetActive(true);
        sessionUI.SetActive(false);

        taskManager.LoadFromJson(currentSession.content);
        taskManager.ToggleConfigPainel(false);

        OculusManager.Instance.ToggleEditMode(false);
        OculusManager.Instance.ToggleARVRMode(false);

        Log("[SessionManager] Arquivo de sessao carregado com sucesso! " + currentSession.name);

        sessionInfoUI.SetActive(false);

        EventManager.TriggerEnterSession();
    }

    public void CreateSession()
    {
        //VRKeyboardManager.Instance.GetUserInputString(CreateSession, "");

        KeyboardManager.Instance.GetInput(CreateSession, null, "");
    }

    public void CreateSession(string sessionName)
    {
        if (sessionName.Trim().Length == 0) return;

        sessionUI.SetActive(false);
        loadingUI.SetActive(true);

        string newSessionName = sessionName.Replace(" ", "_") + ".json";
        string sessionFilePath = $"{Application.persistentDataPath}/{sessionFolder}/{newSessionName}";

        Plan plan = new Plan(sessionName);

        string jsonContent = JsonUtility.ToJson(plan);
        File.WriteAllText(sessionFilePath, jsonContent);

        SessionFile newSession = new SessionFile(newSessionName, "", SessionFile.SessionFileState.Local, jsonContent);

        Debug.Log("[SessionManager] Arquivo de sessao criado com sucesso: " + sessionFilePath);

        OnCreateSessionCompleted(newSession, "");
    }

    public void OnCreateSessionCompleted(SessionFile sessionFile, string fileID)
    {
        files.Add(sessionFile);

        UpdateSessionList(isGuest);
    }

    public void SaveCurrentSession()
    {
        loadingUI.SetActive(true);

        Debug.Log("[SessionManager] - Salvando treinamento ...");

        currentSession.content = JsonUtility.ToJson(taskManager.GetPlan());
        File.WriteAllText(currentSession.filePath, currentSession.content);

        if (!NetworkPhoton.Instance.IsConnected || NetworkPhoton.Instance.IsMasterClient || Application.internetReachability == NetworkReachability.NotReachable || isGuest)
        {
            Debug.Log("[SessionManager] - Sem acesso a internet para salvar a sessao!");

            ResetUI();
            loadingUI.SetActive(false);
        }
        else
        {
            Debug.Log($"[SessionManager] -  Salvando arquivo no servidor ...");

            object[] message = new object[3];
            message[0] = Events.FILE_TRANSFER_EVENT_CODE.UPLOAD;
            message[1] = FileRequestType.Session;
            message[2] = currentSession.filePath;

            EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, message);
        }
    }

    public void ExitSession()
    {
        Log($"[ExitSession] Exit Session");
        EventManager.TriggerExitSession();

        sessionParent.SetActive(false);

        taskManager.SavePlan();

        OculusManager.Instance.ToggleARVRMode(true);
        OculusManager.Instance.ToggleEditMode(true);
        OculusManager.Instance.ResetSession();

        SaveCurrentSession();
    }

    private void SendFile(FileRequestType fileType, string fileName)
    {
        string filePath = Path.Combine(sessionFolderPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.Log($"Arquivo {filePath} nao encontrado!");
            return;
        }

        byte[] fileContent = File.ReadAllBytes(filePath);

        PhotonFileRequestSender sender = Instantiate(senderPrefab, transform).GetComponent<PhotonFileRequestSender>();
        sender.SetSenderInfo(fileName, fileContent);

        senders.Add(sender);
        sender.OnDownloadCompleted += OnUploadCompleted;
        sender.Process();

        //sender.SetSenderInfo()
    }

    private void OnUploadCompleted(PhotonFileRequestSender instance)
    {
        Debug.Log($"[SessionManager] Upload realizado com sucesso!");

        senders.Remove(instance);
        Destroy(instance.gameObject);

        LoadSessionScreen(isGuest);
    }

    private void OnFileRequestReceived(object[] message)
    {
        FileRequestType fileType = (FileRequestType)message[1];
        string fileName = (string)message[2];

        Debug.Log("FileRequestReceived   " + fileType.ToString());

        switch (fileType)
        {
            case FileRequestType.Obj:
                Debug.Log("Upload de obj para o servidor nao implementado!");

                break;
            case FileRequestType.Session:
                SendFile(fileType, fileName);

                break;
        }
    }

    public void DeleteSession()
    {
        Debug.Log($"[SessionManager][DeleteSession] Deletando sessao atual: {currentSession.name} em {currentSession.filePath}");
        if (!File.Exists(currentSession.filePath))
        {
            Debug.Log($"[SessionManager][DeleteSession]       Arquvi nao encontrado!");
        }

        File.Delete(currentSession.filePath);
        sessionInfoUI.SetActive(false);
        Debug.Log($"[SessionManager][DeleteSession] Sessao deletada!");

        UpdateSessionList(isGuest);
    }

    public void BackupSession()
    {
        //TODO
    }

    private void OnDestroy()
    {
        //SaveCurrentSession();
    }

    private void OnEnable()
    {
        //NetworkPhoton.Instance.OnFileTransferRequest.AddListener(OnFileRequestReceived);
    }

    private void OnDisable()
    {
        NetworkPhoton.Instance.OnFileTransferRequest.RemoveListener(OnFileRequestReceived);
    }

    public bool IsGuess { get => isGuest; }

    public class SessionFile 
    {
        public enum SessionFileState { Local, Cloud, NotSync }

        public string name;
        public string filePath;
        public SessionFileState state;
        public string content;

        public SessionFile(string name, string filePath, SessionFileState state, string content)
        {
            this.name = name;
            this.filePath = filePath;
            this.state = state;
            this.content = content;
        }
    }

    private static void Log(string message) => Debug.Log($"[SessionManager]{message}");

}
