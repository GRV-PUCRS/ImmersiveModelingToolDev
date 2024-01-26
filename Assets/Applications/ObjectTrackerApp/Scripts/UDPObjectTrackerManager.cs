using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class UDPObjectTrackerManager : Singleton<UDPObjectTrackerManager>
{
    [SerializeField] private TMP_InputField inputIP;
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private TMP_InputField inputObjectID;

    [SerializeField] private float step = 0.1f;

    private Vector3 position = Vector3.zero;
    private object[] message = new object[5];

    private Socket socket;
    private IPEndPoint endPoint;
    private int currentPort = 10000;

    private void Awake()
    {
        //socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }


    public int GetAvailablePort()
    {
        return currentPort++;
    }


    public void ChangeXValue(bool toIncrease)
    {
        SendMessage();
    }
    
    public void ChangeYValue(bool toIncrease)
    {
        SendMessage();
    }

    public void SendMessage()
    {
        endPoint = new IPEndPoint(IPAddress.Parse(inputIP.text), int.Parse(inputPort.text));

        byte[] message = Encoding.UTF8.GetBytes(inputObjectID.text);
        socket.SendTo(message, endPoint);
    }
}
