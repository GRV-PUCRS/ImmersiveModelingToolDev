using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using static DownloadViaPhoton;
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
    private List<PhotonFileRequestReceiver> receivers;

    private string sessionFolderPath;
    private SessionFile currentSession;

    private List<SessionFile> files;
    private int currentPage = 0;
    private bool isGuest = false;

    private void Start()
    {
        sessionFolderPath= $"{Application.persistentDataPath}/{sessionFolder}";

        receivers = new List<PhotonFileRequestReceiver>();

        if (!Directory.Exists(sessionFolderPath))
        {
            Directory.CreateDirectory(sessionFolderPath);
            Logger.Log("[SessionManager] Diretorio de sessoes criado com sucesso! " + sessionFolderPath);
        }
        else
        {
            Logger.Log("[SessionManager] Diretorio de sessoes encontrado!" + sessionFolderPath);
        }
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
            Logger.Log("Nenhum arquivo recebido do servidor!");
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

        Logger.Log("[SessionManager] Salva arquivo " + instance.bufferedFile.name + " ...");
        File.WriteAllText(sessionFolderPath + "/" + instance.bufferedFile.name, fileContent);
        Logger.Log("[SessionManager] Arquivo salvo com sucesso!");

        instance.OnDownloadCompleted -= OnDownloadCompleted;
        receivers.Remove(instance);
        Destroy(instance.gameObject);

        if (receivers.Count != 0)
        {
            Logger.Log("[SessionManager] Inicia proximo donwload!");
            StartReceiver(receivers[0]);
        }
        else
        {
            Logger.Log("[SessionManager] Fim do download dos objetos!");

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
        Logger.Log("[SessionManager] Inicia Download das sessoes ...");
        NetworkPhoton.Instance.OnFileSearchResponse.AddListener(OnFileSearchResponse);

        EventManager.TriggerSendMessageRequest(Events.FILE_SEARCH_EVENT, new object[2] { Events.FILE_SEARCH_EVENT_CODE.REQUEST, FileRequestType.Session });
    }

    private void GetLocalFiles()
    {
        Logger.Log("[SessionManager] Carrega sessoes locais:");
        // Adiciona arquivos locais
        foreach (string filePath in Directory.GetFiles(sessionFolderPath))
        {
            string fileContent = File.ReadAllText(filePath);
            string fileName = Utils.GetFileNameFromPath(filePath);

            files.Add(new SessionFile(fileName, filePath, SessionFile.SessionFileState.Local, fileContent));
            Logger.Log("[SessionManager]    " + fileName);
        }

        Logger.Log("[SessionManager] Arquivos carregados com sucesso!");
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

            Logger.Log("[SessionManager] Arquivo de sessao encontrado: " + sessionFile.name);
            sessionTemplateInstance = Instantiate(sessionTemplate, sessionTemplateParent).GetComponent<SessionTemplate>();
            sessionTemplateInstance.SetSessionInfo(formatedName, sessionFile.state);
            sessionTemplateInstance.Button.callbacks.AddListener(() => SelectSession(sessionFile, formatedName));
        }

        nextButton.Interactable = endIndex < files.Count;

        Debug.Log("[SessionManager] Finish Update UI");
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

        deleteSessionBTN.Interactable = currentSession.state != SessionFile.SessionFileState.Cloud;
    }

    public void LoadSession()
    {
        sessionParent.SetActive(true);
        sessionUI.SetActive(false);

        taskManager.LoadFromJson(currentSession.content);
        taskManager.ToggleConfigPainel(false);

        OculusManager.Instance.ToggleEditMode(false);
        OculusManager.Instance.ToggleARVRMode(false);
        Logger.Log("[SessionManager] Arquivo de sessao carregado com sucesso! " + currentSession.name);

        sessionInfoUI.SetActive(false);
    }

    public void CreateSession()
    {
        VRKeyboardManager.Instance.GetUserInputString(CreateSession, "");
    }

    public void CreateSession(string sessionName)
    {
        /*
        string sessionFilePath = $"{Application.persistentDataPath}/{sessionFolder}/{sessionName.Replace(" ", "_")}.json";

        Plan plan = JsonUtility.FromJson<Plan>(Resources.Load<TextAsset>(deaultSessionFile).text);
        plan.Name = sessionName;

        File.WriteAllText(sessionFilePath, JsonUtility.ToJson(plan));

        Logger.Log("[SessionManager] Arquivo de sessao criado com sucesso: " + sessionFilePath);
        */
        /*
        sessionUI.SetActive(false);
        loadingUI.SetActive(true);

        string newSessionName = sessionName.Replace(" ", "_") + ".json";
        string jsonContent = Resources.Load<TextAsset>(deaultSessionFile).text;
        SessionFile newSession = new SessionFile(newSessionName, "", "", SessionFile.SessionFileState.Local, jsonContent);

        //GoogleDriveManager.Instance.CreateFileInDrive(newSession, OnCreateSessionCompleted);
        */

        sessionUI.SetActive(false);
        loadingUI.SetActive(true);

        string newSessionName = sessionName.Replace(" ", "_") + ".json";
        string sessionFilePath = $"{Application.persistentDataPath}/{sessionFolder}/{newSessionName}";

        Plan plan = JsonUtility.FromJson<Plan>(Resources.Load<TextAsset>(deaultSessionFile).text);
        plan.Name = sessionName;

        string jsonContent = JsonUtility.ToJson(plan);
        File.WriteAllText(sessionFilePath, jsonContent);

        SessionFile newSession = new SessionFile(newSessionName, "", SessionFile.SessionFileState.Local, jsonContent);

        Logger.Log("[SessionManager] Arquivo de sessao criado com sucesso: " + sessionFilePath);

        OnCreateSessionCompleted(newSession, "");
    }

    public void OnCreateSessionCompleted(SessionFile sessionFile, string fileID)
    {
        files.Add(sessionFile);

        UpdateSessionList(isGuest);
    }

    public void SaveCurrentSession()
    {
        Logger.Log("[SessionManager] - Salvando treinamento em arquivo Json ...");

        //taskManager.SavePlan();
        //string json = JsonUtility.ToJson(taskManager.GetPlan());
        //File.WriteAllText(currentSession.filePath, json);

        if (currentSession.state == SessionFile.SessionFileState.Cloud)
        {
            Logger.Log("[SessionManager] - Arquivo remoto nao pode ser salvo!");
            return;
        }
        else
        {
            //taskManager.SavePlan();
            currentSession.content = JsonUtility.ToJson(taskManager.GetPlan());
            File.WriteAllText(currentSession.filePath, currentSession.content);

            //GoogleDriveManager.Instance.UpdateFileInDrive(currentSession);

            Logger.Log($"[SessionManager] - Treinamento salvo em {currentSession.filePath}");
        }
        
    }

    public void ExitSession()
    {
        taskManager.SavePlan();
        SaveCurrentSession();

        sessionParent.SetActive(false);
        sessionUI.SetActive(true);

        OculusManager.Instance.ToggleARVRMode(true);
        OculusManager.Instance.ToggleEditMode(true);
        OculusManager.Instance.ResetSession();

        UpdateUI();
    }

    public void DeleteSession()
    {
        if (currentSession.state == SessionFile.SessionFileState.Cloud)
        {
            Logger.Log("[DeleteSession] - Arquivo remoto nao pode ser deletado: " + currentSession.name);
            return;
        }

        File.Delete(currentSession.filePath);
        sessionInfoUI.SetActive(false);

        UpdateSessionList(isGuest);
    }

    public void BackupSession()
    {
        //TODO
    }

    private void OnDestroy()
    {
        SaveCurrentSession();
    }

    public class SessionFile 
    {
        public enum SessionFileState { Local, Cloud, NotSync}

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

}
