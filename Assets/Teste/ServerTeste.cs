using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerTeste : MonoBehaviour
{
    public Text log;
    public string room = "grv";


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            NetworkPhoton.Instance.CreateRoom(room);
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            for (int i = 0; i < 150; i++)
            {
                byte[] block = new byte[50000];
                for (int j = 0; j < block.Length; j++)
                {
                    block[j] = 1;
                }

                EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, new object[3] { Events.FILE_TRANSFER_EVENT_CODE.FILE, i, block });

                log.text += "Enviei!!!\n";
                Debug.Log("Enviei!!!");
            }
        }

    }
}
