using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string ip;
    [SerializeField] private int port;

    [Header("References")]
    public Transform objectToUpdate;
    public GameObject uiView;
    public TextMeshProUGUI txtIP;
    public TextMeshProUGUI txtPort;
    public TextMeshProUGUI txtTotalIPs;

    private CancellationTokenSource cancellationToken;

    private Vector3 originPosition;
    private Vector3 receivedPosition;
    private Vector3 receivedRotation;
    private Vector3 receivedScale;
    private bool isReceivingData = false;
    private Socket socket;
    private List<string> avaliableIPs;
    private int currentIpIndex = 0;

    private void Start()
    {
        uiView.SetActive(true);

        avaliableIPs = IPManager.GetIPs(ADDRESSFAM.IPv4);

        if (avaliableIPs.Count == 0)
        {
            Debug.LogError("Nenhum adaptador de rede foi encontrado!");
            enabled = false;
            return;
        }

        currentIpIndex = 0;
        UpdateIP();

        port = UDPObjectTrackerManager.Instance.GetAvailablePort();
        txtPort.text = $"Port: {port}";

        originPosition = OculusManager.Instance.Origin.position;
        cancellationToken = new CancellationTokenSource();

        receivedRotation = Vector3.zero;
        receivedRotation = Vector3.zero;
        receivedScale = Vector3.one;

        EventManager.OnOriginChange += EventManager_OnOriginChange;
        EventManager.OnStageChange += EventManager_OnStageChange;
    }

    private void EventManager_OnStageChange()
    {
        cancellationToken.Cancel();
    }

    private void EventManager_OnOriginChange(Transform obj)
    {
        originPosition = obj.position;
    }

    private void Update()
    {
        if (isReceivingData)
        {
            /*
            objectToUpdate.localPosition = Vector3.zero;
            objectToUpdate.position = originPosition + receivedPosition;
            */
            objectToUpdate.localPosition = receivedPosition;
            objectToUpdate.localEulerAngles = receivedRotation;
            objectToUpdate.localScale = receivedScale;
        }
    }

    public void StartReceiving()
    {
        ReceiveData(cancellationToken.Token);

        transform.parent.localPosition = Vector3.zero;
        transform.localPosition = Vector3.zero;

        uiView.SetActive(false);
    }

    private async void ReceiveData(CancellationToken cts)
    {
        try
        {
            await Task.Run(() =>
            {
                Debug.Log($"Socket {ip} {port}");

                //client = new UdpClient(port, AddressFamily.InterNetwork);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.Bind(endPoint);

                isReceivingData = true;

                while (isReceivingData)
                {
                    try
                    {
                        //byte[] dataByte = client.Receive(ref anyIP);
                        byte[] dataByte = new byte[1024];
                        socket.Receive(dataByte);


                        string data = Encoding.UTF8.GetString(dataByte);

                        //Debug.Log(data);
                        string[] stringSplit = data.Split(';');

                        if (stringSplit.Length > 3)
                            receivedPosition = GetVector(stringSplit, 0);

                        if (stringSplit.Length > 6)
                            receivedRotation = GetVector(stringSplit, 3);

                        if (stringSplit.Length > 9)
                            receivedScale = GetVector(stringSplit, 6);

                        //Debug.Log(receivedPosition);
                    }
                    catch (Exception error)
                    {
                        Debug.Log(error.Message);
                        Debug.Log(error.StackTrace);

                        isReceivingData = false;

                        break;
                    }

                    cts.ThrowIfCancellationRequested();
                }
            });
        }
        catch (OperationCanceledException)
        {
            Debug.Log("UDP Thread Cancelled");

        }catch(Exception error)
        {
            Debug.LogError(error.Message);
        }

        socket.Close();
        uiView.SetActive(true);
    }

    private Vector3 GetVector(string[] substrings, int beginIndex)
    {
        Vector3 vector = new Vector3();

        vector.x = ParseFloat(substrings[beginIndex]);
        vector.y = ParseFloat(substrings[beginIndex + 1]);
        vector.z = ParseFloat(substrings[beginIndex + 2]);

        return vector;
    }

    public void UpdateIP(bool next)
    {
        if (next)
            currentIpIndex = (currentIpIndex + 1) % avaliableIPs.Count;
        else
            currentIpIndex = (avaliableIPs.Count + currentIpIndex - 1) % avaliableIPs.Count;

        UpdateIP();
    }

    public void UpdateIP()
    {
        txtTotalIPs.text = $"({currentIpIndex + 1}/{avaliableIPs.Count})";

        ip = avaliableIPs[currentIpIndex];
        txtIP.text = ip;
    }

    public void StopReceiver()
    {
        isReceivingData = false;
        uiView.SetActive(true);
    }

    public float ParseFloat(string message) => float.Parse(message);

    private void OnDisable()
    {
        isReceivingData = false;
        cancellationToken.Cancel();

        EventManager.OnOriginChange -= EventManager_OnOriginChange;
        EventManager.OnStageChange -= EventManager_OnStageChange;
    }
}
