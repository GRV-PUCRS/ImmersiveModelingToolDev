using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonFileRequestSender : MonoBehaviour
{
    private const int MAX_BLOCK_SIZE = 50000;

    private string fileName;
    private byte[] bytes;
    private float timeout = 3f;
    private int maxTimemoutCounter = 3;
    private bool isTimeout = false;
    private bool confirmation = false;

    public bool isDone = false;
    public Action<PhotonFileRequestSender> OnDownloadCompleted;

    Coroutine timerCoroutine;

    public void SetSenderInfo(string fileName, byte[] bytes)
    {
        this.fileName = fileName;
        this.bytes = bytes;
    }

    public void Process()
    {
        StartCoroutine(SendFileCoroutine(fileName, bytes));
    }

    private void OnFileConfirmation()
    {
        confirmation = true;
    }

    private IEnumerator SendFileCoroutine(string fileName, byte[] fileInBytes)
    {
        //Debug.Log($"[PROCESS({fileName}] Send File Coroutine ...");
        NetworkPhoton.Instance.OnFileTransferConfirmation.AddListener(OnFileConfirmation);

        object[] message = new object[3];
        int messageSize = Mathf.Min(MAX_BLOCK_SIZE, fileInBytes.Length);

        message[0] = Events.FILE_TRANSFER_EVENT_CODE.INFO;
        message[1] = fileName;
        message[2] = fileInBytes.Length;

        //Debug.Log($"[PROCESS({fileName}]    Envia primeira mensagem ...");

        yield return StartCoroutine(WaitForResponse(Events.FILE_TRANSFER_EVENT, message));

        //Debug.Log($"[PROCESS({fileName}]    Primeira mensagem enviada com sucesso");

        message = new object[2];
        message[0] = Events.FILE_TRANSFER_EVENT_CODE.FILE;

        int currentByte = 0;
        int messageSizeSend = 0;
        int nextMessageSize = messageSize;

        while (messageSizeSend < fileInBytes.Length)
        {
            int i = 0;
            byte[] bytes = new byte[nextMessageSize];

            for (; currentByte < messageSizeSend + nextMessageSize; currentByte++)
            {
                bytes[i++] = fileInBytes[currentByte];
            }

            message[1] = bytes;

            //Debug.Log($"[PROCESS({fileName}]  {k-1}  Envia parte da mensagem");
            //EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, message);
            
            yield return StartCoroutine(WaitForResponse(Events.FILE_TRANSFER_EVENT, message));

            if (confirmation)
            {
                //Debug.Log($"[PROCESS({fileName}]        Pedacos enviados corretamente!");

                if (timerCoroutine != null)
                    StopCoroutine(timerCoroutine);
            }
            else
            {
                //Debug.Log($"[PROCESS({fileName}]        Tentativas de envio de arquivo excedidas!");

                yield break;
            }
            
            messageSizeSend += nextMessageSize;
            nextMessageSize = Mathf.Min(messageSize, fileInBytes.Length - messageSizeSend);

            //Debug.Log($"[PROCESS({fileName}]  messageSize({messageSize}) | messageSizeSend({messageSizeSend}) | nextMessageSize({nextMessageSize}) | FileLenght({fileInBytes.Length})");
        }

        yield return new WaitUntil(() => confirmation);

        NetworkPhoton.Instance.OnFileTransferConfirmation.RemoveListener(OnFileConfirmation);
        OnDownloadCompleted?.Invoke(this);

        //Debug.Log($"[PROCESS({fileName}] Arquivo enviado com sucesso!");
    }

    IEnumerator WaitForResponse(byte eventCode, object[] message)
    {
        int timeoutCounter = 0;
        confirmation = false;

        while (!confirmation && timeoutCounter <= maxTimemoutCounter)
        {
            timeoutCounter++;

            EventManager.TriggerSendMessageRequest(eventCode, message);

            isTimeout = false;
            timerCoroutine = StartCoroutine(Timer(timeout));

            //Debug.Log("Espera timeout ou receber confirmacao");
            yield return new WaitUntil(() => confirmation || isTimeout);

            Debug.Log("Sau do loop");
        }

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);


        //Debug.Log("Fim timeout " + confirmation + "  " + isTimeout);
    }

    private IEnumerator Timer(float newTimeout)
    {
        yield return new WaitForSeconds(newTimeout);
        isTimeout = true;

        //Debug.Log("Timer end");
    }
}
