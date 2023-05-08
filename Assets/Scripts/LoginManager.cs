using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Login Parameters")]
    [SerializeField] private string loginCredentialsURL;

    [Header("Screen Parents")]
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject sessionScreen;
    [SerializeField] private GameObject loadingScreen;

    [Header("Session UI")]
    [SerializeField] private SessionManager sessionManager;
    [SerializeField] private TextMeshProUGUI serverTxt;
    [SerializeField] private GameObject loginBtn;

    [Header("Login UI")]
    [SerializeField] private InputField inputFieldLogin; 
    [SerializeField] private InputField inputFieldPassword;
    [SerializeField] private TextMeshProUGUI errorMessageTxt;

    private Dictionary<string, string> credentials;
    private bool isConnectedToServer;

    private void Start()
    {
        credentials = new Dictionary<string, string>();

        LoadLoginScreen();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            /*
            inputFieldLogin.text = "Grv";
            inputFieldPassword.text = "789";
            */


            inputFieldLogin.text = "grv";
            inputFieldPassword.text = "123";
            Login(false);
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            Login(true);
        }
    }

    public void LoadLoginScreen()
    {
        loginScreen.SetActive(true);
        sessionScreen.SetActive(false);
        loadingScreen.SetActive(false);
        errorMessageTxt.gameObject.SetActive(false);
        OculusManager.Instance.ToggleEditMode(true);

        //LoadCredentials();
    }

    public void SetErrorMessage(bool visible, string message)
    {
        errorMessageTxt.gameObject.SetActive(visible);
        errorMessageTxt.text = message;
    }

    public void LoadCredentials()
    {
        DownloadManager.Instance.DownloadStringFile(loginCredentialsURL, ParseCredentialsFile);
    }

    public void GetLoginFromUser()
    {
        LoadCredentials();

        errorMessageTxt.gameObject.SetActive(false);
        inputFieldLogin.text = "";
        inputFieldPassword.text = "";

        loginScreen.SetActive(true);
        sessionScreen.SetActive(false);
    }

    private void ParseCredentialsFile(string fileContent)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SetErrorMessage(true, "ERRO: Sem acesso a Internet");
            Logger.Log("Erro ao baixar as credenciais do servidor!");

            return;
        }

        credentials = new Dictionary<string, string>();

        string[] lines = fileContent.Replace("\n", "").Split(';');

        foreach (string line in lines)
        {
            string[] info = line.Split(',');

            if (info.Length != 2) continue;

            credentials.Add(info[0].ToLower(), info[1]);
        }

        Logger.Log("Credenciais: " + credentials.Keys);
    }

    public void Login(bool guest)
    {
        if (guest)
        {
            SetLoginInfo(true);
        }
        else
        {
            Logger.Log("Tentendo entrar na Room " + inputFieldLogin.text);
            NetworkPhoton.Instance.JoinRoom(inputFieldLogin.text);
        }

        /*
        string login = inputFieldLogin.text.ToLower();
        string password = inputFieldPassword.text;

        if (guest || (credentials.ContainsKey(login) && credentials[login].Equals(password)))
        {
            Debug.Log("Login realizado com sucesso!");

            loginScreen.SetActive(false);
            sessionScreen.SetActive(true);

            serverTxt.text = guest ? "GUEST" : login.ToUpper();
            loginBtn.SetActive(false);

            sessionManager.LoadSessionScreen(guest);
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SetErrorMessage(true, "ERRO: Sem acesso a Internet");
                Logger.Log("Erro ao baixar as credenciais do servidor!");
            }
            else
            {
                SetErrorMessage(true, "Erro: Credenciais inválidas!");
                Logger.Log("Erro ao baixar as credenciais do servidor!");
            }
        }
        */
    }

    public void GetLoginString()
    {
        VRKeyboardManager.Instance.GetUserInputString(HandleLoginInput, "");
    }

    public void GetPasswordString()
    {
        VRKeyboardManager.Instance.GetUserInputString(HandlePasswordInput, "", TMP_InputField.ContentType.Password);
    }

    private void HandleLoginInput(string newString)
    {
        inputFieldLogin.text = newString;
    }

    private void HandlePasswordInput(string newString)
    {
        inputFieldPassword.text = newString;
    }

    public bool IsConnectedToServer { get => isConnectedToServer; }

    private void OnJoinedRoom(int playerCount)
    {
        SetLoginInfo(false);
    }

    private void SetLoginInfo(bool isGuest)
    {
        Logger.Log("Login feito com sucesso! GUEST = " + isGuest);

        loginScreen.SetActive(false);
        sessionScreen.SetActive(true);

        serverTxt.text = isGuest ? "GUEST" : inputFieldLogin.text.ToUpper();
        loginBtn.SetActive(false);

        sessionManager.LoadSessionScreen(isGuest);
    }

    private void OnJoinRoomFailed(string message)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SetErrorMessage(true, "ERRO: Sem acesso a Internet");
            Logger.Log("Sem acesso a internet!");
        }
        else
        {
            SetErrorMessage(true, "Servidor nao encontrado!");
            Logger.Log("Servidor nao encontrado!");
        }

    }

    private void OnEnable()
    {
        if(NetworkPhoton.Instance == null)
        {
            Debug.Log("NetworkPhoton not found!");
        }
        else
        {
            NetworkPhoton.Instance.OnJoinedRoomEvent.AddListener(OnJoinedRoom);
            NetworkPhoton.Instance.OnJoinRoomFailedEvent.AddListener(OnJoinRoomFailed);
        }
    }
}
