using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PhotonServerManager;

public class PhotonFileRequestReceiver : MonoBehaviour
{
    private byte[] buffer;
    private int fileSize;
    private string fileName;
    private int currentByteCount = -1;
    private FileRequestType fileType;

    public bool isDone = false;
    public BufferedFile bufferedFile;
    public Action<PhotonFileRequestReceiver> OnDownloadCompleted;

    private Action<float> progressCallback;

    public void SetReceiverInfo(string fileName, FileRequestType fileType, Action<float> progressCallback)
    {
        this.fileName = fileName;
        this.fileType = fileType;
        this.progressCallback = progressCallback;

        UpdateStatus(0);
    }

    public void Process()
    {
        //Logger.Log($"============ PROCESS    " + fileName + "=========");

        NetworkPhoton.Instance.OnFileTransferInfo.AddListener(ReceiveFileInfo);
        object[] message = new object[3];

        message[0] = Events.FILE_TRANSFER_EVENT_CODE.REQUEST;
        message[1] = fileType;
        message[2] = fileName;

        EventManager.TriggerSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, message);
        //Logger.Log($"================  PROCESS =======================");
    }

    private void ReceiveFileInfo(object[] message)
    {
        //Logger.Log($"============ Receive File Info ({(string)message[1]})=============");

        currentByteCount = 0;
        fileName = (string)message[1];
        fileSize = (int)message[2];

        buffer = new byte[fileSize];

        //Logger.Log($"[PROCESS({fileName})] Envia confirmacao!");
        EventManager.TriggerSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.CONFIRMATION });

        NetworkPhoton.Instance.OnFileTransferInfo.RemoveListener(ReceiveFileInfo);
        NetworkPhoton.Instance.OnFileTransferFile.AddListener(HandleReceiveFile);
        //Logger.Log($"============================================");
    }

    private void HandleReceiveFile(object[] message)
    {
        //Logger.Log($"[PROCESS({fileName})] Recebe parte do arquivo");

        byte[] block = (byte[])message[1];

        //Debug.Log($"{(Events.FILE_TRANSFER_EVENT_CODE)message[0]}    {message.Length}");

        //Logger.Log($"[PROCESS({fileName})]   {(int)message[2]}   block.Length({block.Length}) currentByteCount({currentByteCount})");

        for (int i = 0; i < block.Length; i++)
        {
            buffer[currentByteCount++] = block[i];
        }
        
        //Logger.Log($"[PROCESS({fileName})]      Envia confirmacao!");
        EventManager.TriggerSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.CONFIRMATION });
        
        // Finish
        if (currentByteCount == buffer.Length)
        {
            //Logger.Log($"[PROCESS({fileName})]          Arquivo final recebido com sucesso!");

            //Logger.Log($"[PROCESS({fileName})]      Envia confirmacao!");
            EventManager.TriggerSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.CONFIRMATION });

            NetworkPhoton.Instance.OnFileTransferFile.RemoveListener(HandleReceiveFile);

            UpdateStatus(100);
            FinishProcess();
        }
        else
        {
            UpdateStatus(Mathf.RoundToInt((currentByteCount / (float)fileSize) * 100));
        }
    }

    public void UpdateStatus(int newPercet)
    {
        progressCallback?.Invoke(newPercet);
    }

    private void FinishProcess()
    {
        bufferedFile = new BufferedFile(fileName, buffer);

        isDone = true;
        OnDownloadCompleted?.Invoke(this);
    }

    public string FileName { get => fileName; }
}
