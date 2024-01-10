using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
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

    private bool isConnectedToServer;
    private bool isGuest = false;

    private void Start()
    {
        LoadLoginScreen();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
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

    public void GetLoginFromUser()
    {
        errorMessageTxt.gameObject.SetActive(false);
        inputFieldLogin.text = "";
        inputFieldPassword.text = "";

        loginScreen.SetActive(true);
        sessionScreen.SetActive(false);
    }

    public void Login(bool guest)
    {
        isGuest = guest;

        if (guest)
        {
            SetLoginInfo(true);
            //NetworkPhoton.Instance.CreateRoom("Local");
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SetErrorMessage(true, "ERRO: Sem acesso a Internet");
                Debug.Log("Erro ao baixar as credenciais do servidor!");
            }
            else
            {
                if (NetworkPhoton.Instance.IsMasterClient) ExitRoom();


                Debug.Log("Tentendo entrar na Room " + inputFieldLogin.text);
                NetworkPhoton.Instance.JoinRoom(inputFieldLogin.text + inputFieldPassword.text);
            }
        }
    }

    public void ExitRoom()
    {
        NetworkPhoton.Instance.ExitRoom();

        loginScreen.SetActive(true);
        sessionScreen.SetActive(false);

        errorMessageTxt.gameObject.SetActive(false);
    }

    public void GetLoginString()
    {
        //VRKeyboardManager.Instance.GetUserInputString(HandleLoginInput, "");
        KeyboardManager.Instance.GetInput(HandleLoginInput, null, inputFieldLogin.text);
    }

    public void GetPasswordString()
    {
        //VRKeyboardManager.Instance.GetUserInputString(HandlePasswordInput, "", TMP_InputField.ContentType.Password);
        //VRKeyboardManager.Instance.GetUserInputString(HandlePasswordInput, "");
        KeyboardManager.Instance.GetInput(HandlePasswordInput, null, inputFieldPassword.text);
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
        SetLoginInfo(isGuest);
    }

    private void SetLoginInfo(bool isGuest)
    {
        Debug.Log("Login feito com sucesso! GUEST = " + isGuest);

        loginScreen.SetActive(false);
        sessionScreen.SetActive(true);

        serverTxt.text = isGuest ? "Local" : inputFieldLogin.text.ToUpper();
        loginBtn.SetActive(false);

        sessionManager.LoadSessionScreen(isGuest);
    }

    private void OnJoinRoomFailed(string message)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SetErrorMessage(true, "ERRO: Sem acesso a Internet");
            Debug.Log("Sem acesso a internet!");
        }
        else
        {
            SetErrorMessage(true, "Servidor nao encontrado ou credenciais invalidas!");
            Debug.Log("Servidor nao encontrado!");
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
