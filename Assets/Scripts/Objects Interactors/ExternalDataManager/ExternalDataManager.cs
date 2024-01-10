using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ExternalDataManager : Singleton<ExternalDataManager>
{
    [Header("Config")]
    [SerializeField] private string ip = "192.168.1.213";
    [SerializeField] private int port = 10000;

    private List<string> receivedData;
    private List<IExternalDataListener> listeners;
    private Socket socket;
    private bool dataChanged = false;

    public event Action<bool> OnConnectionChanged;

    private bool connected = false;
    private bool socketConnect = false;

    private void Awake()
    {
        receivedData = new List<string>();
        listeners = new List<IExternalDataListener>();
    }

    private void Update()
    {
        if (dataChanged)
        {
            dataChanged = false;
            Debug.Log($"[ExternalDataManager] Data Changed");

            foreach (string data in receivedData)
            {
                listeners.ForEach(l => l.ReceivedData(data));
            }
        }

        if (connected != socketConnect)
        {
            socketConnect = connected;
            OnConnectionChanged?.Invoke(connected);
        }
    }

    private async void ReceiveData()
    {
        try
        {
            await Task.Run(() =>
            {
                Debug.Log($"[ExternalDataManager] Socket {ip} {port}");

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.Bind(endPoint);

                connected = true;

                while (connected)
                {
                    try
                    {
                        Debug.Log("[ExternalDataManager] Waiting for data ...");
                        //byte[] dataByte = client.Receive(ref anyIP);
                        byte[] dataByte = new byte[1024];
                        socket.Receive(dataByte);

                        Debug.Log(dataByte);

                        if (dataByte.Length == 0) continue;

                        string data = Encoding.UTF8.GetString(dataByte);

                        Debug.Log($"[ExternalDataManager] Message {data}");

                        receivedData = JsonConvert.DeserializeObject<List<string>>(data);

                        dataChanged = receivedData != null;
                    }
                    catch (Exception error)
                    {
                        Debug.Log(error.Message);
                        Debug.Log(error.StackTrace);

                        connected = false;

                        break;
                    }
                }
            });
        }
        catch (OperationCanceledException exception)
        {
            Debug.Log("[ExternalDataManager] UDP Thread Cancelled", this);

        }catch (Exception generalExcpetion)
        {
            Debug.Log($"[ExternalDataManager] Error: {generalExcpetion.Message}", this);
        }

        connected = false;
        socket.Close();
        Debug.Log($"[ExternalDataManager] Socket closed", this);
    }

    public bool AddListener(IExternalDataListener listener)
    {
        if (listeners.Contains(listener)) return false;

        listeners.Add(listener);

        Debug.Log($"[ExternalDataManager] Add Listener", this);

        return true;
    }

    public void RmvListener(IExternalDataListener listener)
    {
        listeners.Remove(listener);

        Debug.Log($"[ExternalDataManager] Rmv Listener", this);
    }

    public void ConnectSocket(string ip, int port)
    {
        Debug.Log($"[ExternalDataManager] Try init socket in {ip}:{port}...", this);
        this.ip = ip;
        this.port = port;

        CloseSocket();
        ReceiveData();
    }

    private void CloseSocket()
    {
        Debug.Log($"[ExternalDataManager] Close socket", this);

        connected = false;
        dataChanged = false;

        if (socket != null) socket.Close();
    }

    private void EventManager_OnExitSession()
    {
        Debug.Log($"[ExternalDataManager] Exit Session", this);
        CloseSocket();
    }


    private void EventManager_OnEnterSession()
    {
        Debug.Log($"[ExternalDataManager] Enter in session", this);
    }

    private void OnEnable()
    {
        EventManager.OnEnterSession += EventManager_OnEnterSession;
        EventManager.OnExitSession += EventManager_OnExitSession;
        EventManager.OnStageChange += EventManager_OnStageChange;
    }

    private void OnDisable()
    {
        socket.Close();

        EventManager.OnEnterSession -= EventManager_OnEnterSession;
        EventManager.OnExitSession += EventManager_OnExitSession;
        EventManager.OnStageChange -= EventManager_OnStageChange;
    }

    private void EventManager_OnStageChange()
    {
        TextReceiver.idCounter = 0;
    }
}
