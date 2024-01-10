using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

public class FileTransferSender
{
    private const int MAX_BLOCK_SIZE = 50000;

    private string fileName;
    private byte[] bytes;
    private int timeout = 500;   //milisegundos
    private int maxTimemoutCounter = 3;
    private bool isTimeout = false;
    private bool confirmation = false;

    public bool isDone = false;
    public Action<FileTransferSender> OnDownloadCompleted;

    private CancellationTokenSource cancellationToken;

    public FileTransferSender(string fileName, byte[] bytes)
    {
        this.fileName = fileName;
        this.bytes = bytes;

        cancellationToken = new CancellationTokenSource();
    }

    public void Start()
    {
        Task.Run(SendFile);
    }

    private async void SendFile()
    {
        Debug.Log($"Comeca Envio de Arquivo {fileName} ...");

        NetworkPhoton.Instance.OnFileTransferConfirmation.AddListener(OnFileConfirmation);

        object[] message = new object[3];
        int messageSize = Mathf.Min(MAX_BLOCK_SIZE, bytes.Length);

        message[0] = Events.FILE_TRANSFER_EVENT_CODE.INFO;
        message[1] = fileName;
        message[2] = bytes.Length;

        EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, message);

        await WaitForResponse(timeout);

        if (isTimeout)
        {
            Debug.Log($"Timeout ao tentar receber resposta do Evento File Transfer Info");
            cancellationToken.Cancel();
        }

        message = new object[2];
        message[0] = Events.FILE_TRANSFER_EVENT_CODE.FILE;

        int currentByte = 0;
        int messageSizeSend = 0;
        int nextMessageSize = messageSize;

        while (messageSizeSend < bytes.Length)
        {
            int i = 0;
            byte[] block = new byte[nextMessageSize];

            for (; currentByte < messageSizeSend + nextMessageSize; currentByte++)
            {
                block[i++] = bytes[currentByte];
            }

            message[1] = block;

            EventManager.TriggerSendMessageRequest(Events.FILE_TRANSFER_EVENT, message);

            await WaitForResponse(timeout);

            if (isTimeout)
            {
                Debug.Log("Timeout ao tentar enviar parte do arquivo.");
                cancellationToken.Cancel();
            }

            messageSizeSend += nextMessageSize;
            nextMessageSize = Mathf.Min(messageSize, bytes.Length - messageSizeSend);
        }

        /*
        await WaitForResponse(timeout);

        if (isTimeout)
        {
            Debug.Log("Timeout ao tentar enviar a ultima parte do arquivo.");
            cancellationToken.Cancel();
        }
        */

        NetworkPhoton.Instance.OnFileTransferConfirmation.RemoveListener(OnFileConfirmation);
        OnDownloadCompleted?.Invoke(this);
    }

    private void OnFileConfirmation()
    {
        confirmation = true;
    }

    private async Task WaitForResponse(int timeout)
    {
        isTimeout = false;
        confirmation = false;

        CancellationTokenSource timeoutCancellationToken = new CancellationTokenSource();
        Task[] tasks = { Task.Run(() => WaitForComfirmation(timeoutCancellationToken.Token)), Task.Run(() => Timer(maxTimemoutCounter, timeout, timeoutCancellationToken.Token)) };

        int index = Task.WaitAny(tasks);

        timeoutCancellationToken.Cancel();
    }

    private async Task WaitForComfirmation(CancellationToken token)
    {
        while (!confirmation)
        {
            await Task.Delay(100, token);
        }
    }

    private async Task Timer(int maxTimeoutCounter, int timeout, CancellationToken token)
    {
        int timeoutCounter = 0;

        while (timeoutCounter < maxTimeoutCounter)
        {
            await Task.Delay(timeout, token);
            timeoutCounter++;
        }

        isTimeout = true;
    }
}
