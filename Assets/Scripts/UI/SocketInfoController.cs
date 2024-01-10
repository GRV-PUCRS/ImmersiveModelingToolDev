using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SocketInfoController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI txtIP;
    [SerializeField] private TextMeshProUGUI txtPort;
    [SerializeField] private TextMeshProUGUI txtTotalIPs;
    [SerializeField] private GameObject imgStatusGreen;

    private List<string> avaliableIPs = new List<string>();
    private int currentIpIndex = 0;

    private void Start()
    {
        avaliableIPs = IPManager.GetIPs(ADDRESSFAM.IPv4);

        if (avaliableIPs.Count == 0)
        {
            enabled = false;
            return;
        }

        currentIpIndex = 0;
        UpdateIP();

    }

    private void Instance_OnConnectionChanged(bool value)
    {
        imgStatusGreen.SetActive(value);
    }

    public void UpdateIP(bool next)
    {
        if (next)
            currentIpIndex = (currentIpIndex + 1) % avaliableIPs.Count;
        else
            currentIpIndex = (avaliableIPs.Count + currentIpIndex - 1) % avaliableIPs.Count;

        UpdateIP();
    }

    public void SetPort(int port)
    {
        txtPort.text = $"{port}";
    }

    public void UpdateIP()
    {
        txtTotalIPs.text = $"({currentIpIndex + 1}/{avaliableIPs.Count})";

        txtIP.text = $"{avaliableIPs[currentIpIndex]}";
    }

    public void SetIP(string newIP)
    {
        int index = avaliableIPs.FindIndex(ip => ip.Equals(newIP));

        if (index == -1) return;

        currentIpIndex = index;

        txtTotalIPs.text = $"({currentIpIndex + 1}/{avaliableIPs.Count})";

        txtIP.text = $"{avaliableIPs[currentIpIndex]}";
    }

    public void ChangePort()
    {
        KeyboardManager.Instance.GetInput(SetPort, null);
    }

    private void SetPort(string stringPort)
    {
        try
        {
            int port = int.Parse(stringPort);

            txtPort.text = $"{stringPort}";
        
        }catch (Exception error)
        {
            Debug.LogError(error.Message, this);
        }
    }

    public void Connect()
    {
        ExternalDataManager.Instance.ConnectSocket(txtIP.text, int.Parse(txtPort.text));
    }

    private void OnEnable()
    {
        ExternalDataManager.Instance.OnConnectionChanged += Instance_OnConnectionChanged;
    }

    private void OnDisable()
    {
        ExternalDataManager.Instance.OnConnectionChanged -= Instance_OnConnectionChanged;
    }
}
