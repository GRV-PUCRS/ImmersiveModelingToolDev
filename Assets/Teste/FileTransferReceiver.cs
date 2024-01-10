using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static PhotonServerManager;

public class FileTransferReceiver
{
    private byte[] buffer;
    private int fileSize;
    private string fileName;
    private int currentByteCount = -1;
    private FileRequestType fileType;

    public bool isDone = false;
    public BufferedFile bufferedFile;
    private Action<FileTransferReceiver> successCallback;
    private Action<float> progressCallback;

    public FileTransferReceiver(string fileName, Action<FileTransferReceiver> successCallback, Action<float> progressCallback)
    {
        this.fileName = fileName;
        this.successCallback = successCallback;
        this.progressCallback = progressCallback;

        UpdateStatus(0);
    }

    public void Start()
    {
        Task.Run(ReceiveFile);
    }

    public async Task ReceiveFile()
    {
        Debug.Log("Receiver: Comeca a receber arquivo");
        NetworkPhoton.Instance.OnFileTransferInfo.AddListener(ReceiveFileInfo);
        object[] message = new object[3];

        message[0] = Events.FILE_TRANSFER_EVENT_CODE.REQUEST;
        message[1] = fileType;
        message[2] = fileName;

        EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, message);

        while (!isDone)
        {
            await Task.Delay(100);
        }
    }

    private void ReceiveFileInfo(object[] message)
    {
        Debug.Log("Receiver: Recebe Info");

        NetworkPhoton.Instance.OnFileTransferInfo.RemoveListener(ReceiveFileInfo);

        currentByteCount = 0;
        fileName = (string)message[1];
        fileSize = (int)message[2];

        buffer = new byte[fileSize];

        SendConfirmation();

        NetworkPhoton.Instance.OnFileTransferFile.AddListener(HandleReceiveFile);
    }

    private void HandleReceiveFile(object[] message)
    {
        Debug.Log("Receiver: Recebe parte do arquivo");

        byte[] block = (byte[])message[1];

        for (int i = 0; i < block.Length; i++)
        {
            buffer[currentByteCount++] = block[i];
        }

        SendConfirmation();

        // Finish
        if (currentByteCount == buffer.Length)
        {
            UpdateStatus(100);
            FinishProcess();
        }
        else
        {
            UpdateStatus(Mathf.RoundToInt((currentByteCount / (float)fileSize) * 100));
        }
    }

    public void SendConfirmation()
    {
        EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.CONFIRMATION });
    }

    public void UpdateStatus(int newPercet)
    {
        progressCallback?.Invoke(newPercet);
    }

    private void FinishProcess()
    {
        NetworkPhoton.Instance.OnFileTransferFile.RemoveListener(HandleReceiveFile);

        bufferedFile = new BufferedFile(fileName, buffer);

        successCallback?.Invoke(this);

        isDone = true;
        Debug.Log("Receiver: Fim!");
    }

    public string FileName { get => fileName; }
}
