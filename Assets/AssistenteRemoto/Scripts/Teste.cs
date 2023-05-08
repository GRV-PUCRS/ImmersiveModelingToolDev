using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static DownloadViaPhoton;
using static PhotonServerManager;
using static SessionManager;

public class Teste : MonoBehaviour
{
    [Header("Photon Config")]
    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject receiverPrefab;
    private List<PhotonFileRequestReceiver> receivers = new List<PhotonFileRequestReceiver>();
    public string room = "grv";

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            NetworkPhoton.Instance.JoinRoom(room);
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            DownloadObjects();
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            NetworkPhoton.Instance.OnSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.FILE });
            Debug.Log("Envia mensagem de arquivo");


            NetworkPhoton.Instance.OnSendMessageToServerRequest(Events.FILE_TRANSFER_EVENT, new object[1] { Events.FILE_TRANSFER_EVENT_CODE.CONFIRMATION });
            Debug.Log("Envia mensagem de confirmacao");
        }
    }

    private void OnFileSearchResponse(object[] message)
    {
        Logger.Log($"[ObjectManager] Recebe lista de arquivos do servidor!");

        NetworkPhoton.Instance.OnFileSearchResponse.RemoveListener(OnFileSearchResponse);

        string[] files = (string[])message[1];

        if (files.Length == 0)
        {
            Logger.Log("[ObjectManager] Nenhum arquivo recebido para download");
            return;
        }

        Logger.Log($"[ObjectManager]    Arquivos recebidos: " + files.Length);

        foreach (string file in files)
        {
            PhotonFileRequestReceiver receiver = Instantiate(receiverPrefab, requestParent).GetComponent<PhotonFileRequestReceiver>();
            receiver.SetReceiverInfo(file, FileRequestType.Obj, null);
            receivers.Add(receiver);
        }

        Logger.Log($"[ObjectManager]    Inicia download individuais");

        InstantiateReceiver();
    }

    private void OnDownloadFinished(PhotonFileRequestReceiver instance)
    {
        Logger.Log("=> Salva" + instance.bufferedFile.name);

        string path = Path.Combine(Application.persistentDataPath, "bundles");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllBytes(Path.Combine(path, instance.bufferedFile.name), instance.bufferedFile.data);

        receivers.RemoveAt(0);
        instance.OnDownloadCompleted -= OnDownloadFinished;
        Destroy(instance.gameObject);

        if (receivers.Count != 0)
        {
            Logger.Log($"[ObjectManager]    Inicia proximo download ...");
            InstantiateReceiver();
        }
        else
        {
            Logger.Log($"[ObjectManager]    Termina de baixar os objetos!");
        }
    }


    private void InstantiateReceiver()
    {
        receivers[0].Process();
        receivers[0].OnDownloadCompleted += OnDownloadFinished;
    }

    public void DownloadObjects()
    {
        Logger.Log($"[ObjectManager] Inicia download dos objetos:");

        NetworkPhoton.Instance.OnFileSearchResponse.AddListener(OnFileSearchResponse);

        EventManager.TriggerSendMessageRequest(Events.FILE_SEARCH_EVENT, new object[2] { Events.FILE_SEARCH_EVENT_CODE.REQUEST, FileRequestType.Obj });
    }
}
