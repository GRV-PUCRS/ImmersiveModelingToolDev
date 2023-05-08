using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIManager : MonoBehaviour
{
    public static ClientUIManager Instance { get; private set; }

    [SerializeField] private Text statusText;
    [SerializeField] public Text infoText;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.OnSocketConnectionChange += OnSocketConnectionChange;
    }

    private void OnSocketConnectionChange(bool _isConnected)
    {
        //statusText.text = "Status: " + ((_isConnected) ? "Conectado" : "Disconectado");
    }

    public static void SetStatus(string message)
    {
        //Instance.statusText.text = message;
    }
}
