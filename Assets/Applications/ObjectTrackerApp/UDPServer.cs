using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string ip = "192.168.1.213";
    [SerializeField] private int port = 10000;

    private Vector3 originPosition;
    private Vector3 receivedPosition;
    private bool isReceivingData = false;
    private Socket socket;

    private void Start()
    {
        ReceiveData();
    }


    private async void ReceiveData()
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

                        Debug.Log(dataByte);

                        if (dataByte.Length == 0) continue;

                        string data = Encoding.UTF8.GetString(dataByte);

                        Debug.Log($"Message {data}");

                    }
                    catch (Exception error)
                    {
                        Debug.Log(error.Message);
                        Debug.Log(error.StackTrace);

                        isReceivingData = false;

                        break;
                    }
                }
            });
        }
        catch (OperationCanceledException)
        {
            Debug.Log("UDP Thread Cancelled");
        }
        socket.Close();
    }


    private void OnDisable()
    {
        socket.Close();
    }
}
