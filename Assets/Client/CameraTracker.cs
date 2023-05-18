using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    private bool connected = false;
    private object[] message;

    private void Awake()
    {
        message = new object[4];

        // ID do objeto
        message[0] = 0;
        message[1] = "Camera";
    }

    private void Update()
    {
        if (connected)
        {
            // Envia Posicao
            message[2] = transform.position;
            // Envia Rotacao
            message[3] = transform.rotation.eulerAngles;

            EventManager.TriggerSendMessageRequest(Events.UPDATE_CAMERA, message);
        }
    }

    public void OnConnectedToServer()
    {
        connected = true;
    }

    public void OnDisconnectedFromServer()
    {
        connected = false;
    }
}
