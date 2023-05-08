using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientTeste : MonoBehaviour
{
    public Text log;
    public string room = "grv";

    private void Start()
    {
        NetworkPhoton.Instance.OnFileTransferFile.AddListener(OnFileTransfer);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            NetworkPhoton.Instance.JoinRoom(room);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            object[] message = new object[3];
            message[0] = Events.FILE_SEARCH_EVENT_CODE.REQUEST;
            message[1] = Events.FILE_TRANSFER_EVENT_CODE.FILE;


            NetworkPhoton.Instance.OnFileSearchResponse.AddListener(OnFileSearchResponse);

            Debug.Log("Envia solicitacao de arquivos");
            EventManager.TriggerSendMessageToServerRequest(Events.FILE_SEARCH_EVENT, message);
        }
    }

    private void OnFileSearchResponse(object[] message)
    {
        Debug.Log("Recebe lista de arquivos");
        string[] files = (string[])message[1];

        foreach (string file in files)
        {
            Debug.Log(file);
        }
    }

    private void OnFileTransfer(object[] message)
    {
        log.text += "Recebi!!!      " + (int)message[1] + "\n";
        Debug.Log("Recebi!!!        " + (int)message[1]);
    }
}
