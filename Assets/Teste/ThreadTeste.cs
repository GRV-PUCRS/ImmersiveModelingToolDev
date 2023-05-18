using UnityEngine;
using System.IO;
using static Events;
using System.Threading.Tasks;
using System.Threading;

public class ThreadTeste : MonoBehaviour
{
    FileTransferSender sender;
    FileTransferReceiver receiver;

    public bool isServer;

    private void Start()
    {
        NetworkPhoton.Instance.OnFileTransferRequest.AddListener(OnFileRequestReceived);

        //Task.Run(Teste);
        //Debug.Log("AQUI!");
    }

    public async Task Teste()
    {
        int index = -1;

        CancellationTokenSource token = new CancellationTokenSource();
        index = Task.WaitAny(new Task[] { Task.Run(() => T1(token.Token), token.Token), Task.Run(() => T2(token.Token), token.Token) });

        token.Cancel();

        Debug.Log($"Index = {index}");
    }

    public async Task T1(CancellationToken token)
    {
        await Task.Delay(1000, token);
        Debug.Log("T1 Terminou");
    }

    public async Task T2(CancellationToken token)
    {
        await Task.Delay(3000, token);
        Debug.Log("T2 Terminou");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (isServer)
            {
                NetworkPhoton.Instance.CreateRoom("temp");
            }
            else
            {
                NetworkPhoton.Instance.JoinRoom("temp");
            }
        }

        if (!isServer && Input.GetKeyUp(KeyCode.P))
        {
            string fileName = "alicate";

            receiver = new FileTransferReceiver(fileName, (FileTransferReceiver instance) =>
            {
                File.WriteAllBytes("D:/Pastas_Pessoais/ViniciusChrisosthemos/Temp/Client/alicate", instance.bufferedFile.data);
                Debug.Log("Salva arquivo");
            }, null);

            receiver.Start();
        }
    }

    private void OnFileRequestReceived(object[] message)
    {
        FILE_SEARCH_EVENT_CODE requestType = (FILE_SEARCH_EVENT_CODE)message[1];

        switch (requestType)
        {
            case FILE_SEARCH_EVENT_CODE.REQUEST:
                string filePath = "D:/Pastas_Pessoais/ViniciusChrisosthemos/Temp/Server/alicate";

                sender = new FileTransferSender(filePath, File.ReadAllBytes(filePath));
                sender.OnDownloadCompleted += (FileTransferSender result) => Debug.Log("Arquivo enviado com sucesso!");
                sender.Start();

                break;

            default:
                break;
        }
    }


}