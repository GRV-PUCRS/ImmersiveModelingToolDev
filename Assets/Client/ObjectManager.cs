using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static PhotonServerManager;
using TMPro;
using UnityEngine.UI;

public class ObjectManager : Singleton<ObjectManager>
{
    [Header("Referencias")]
    [SerializeField] private Transform objectsParent;

    [Header("Paths")]
    [SerializeField] private string assetFolder = "bundles";
    [SerializeField] private string filesFolder = "files";

    [Header("Fixed Objects")]
    [SerializeField] private List<GameObject> _fixedObjects;

    [Header("Photon Config")]
    [SerializeField] private Transform requestParent;
    [SerializeField] private GameObject receiverPrefab;
    private List<PhotonFileRequestReceiver> receivers;

    [Header("Events")]
    public Action OnObjectDownloaded;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtDownloadedFileName;
    [SerializeField] private TextMeshProUGUI txtDownloadCount;
    [SerializeField] private Slider sliderProgress;
    [SerializeField] private GameObject progressView;

    private Dictionary<string, GameObject> assetsObject;
    private int totalObjectToDownload = 0;
    private string assetFolderPath;

    private void Awake()
    {
        assetFolderPath = Path.Combine(Application.persistentDataPath, assetFolder);
        assetsObject = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        receivers = new List<PhotonFileRequestReceiver>();

        AddFixedObjects();

        if (!Directory.Exists(assetFolderPath))
        {
            Directory.CreateDirectory(assetFolderPath);
        }

        LoadAssetBundles();
    }

    public void ResetObjects()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        assetsObject = new Dictionary<string, GameObject>();
    }

    public void DeleteObjects()
    {
        Utils.DeleteFolderContent(assetFolderPath);
    }

    private void LoadAssetBundles()
    {
        Debug.Log("[ObjectManager] Carrega Asset Bundles ...");

        foreach (string file in Directory.GetFiles(assetFolderPath))
        {
            GameObject instance = null;

            // Debug.Log($"==> {ExtensionAllowed.IsAllowedExtension(file)} {file}");
            if (!ExtensionAllowed.IsAllowedExtension(file))
            {
                AssetBundle assetBundle = null;

                try
                {
                    Debug.Log("[ObjectManager]     Tenta carregar " + file);

                    assetBundle = AssetBundle.LoadFromFile(file);
                    instance = assetBundle.LoadAsset<GameObject>(assetBundle.GetAllAssetNames()[0]);

                    instance.AddComponent<DragUI>();
                    instance.AddComponent<InteractableObject>();

                    instance.SetActive(objectsParent);
                    instance.name = assetBundle.name;

                    assetsObject.Add(assetBundle.name, instance);

                    assetBundle.Unload(false);

                    Debug.Log("[ObjectManager]     Asset carregado com sucesso!");
                }
                catch (Exception error)
                {
                    Debug.Log("[ObjectManager]     Erro ao carregar Asset: " + error.Message);

                    if (assetBundle != null)
                        assetBundle.Unload(false);

                    if (instance != null)
                        Destroy(instance);
                }
            }
        }

        Debug.Log("[ObjectManager] Termina de carregar Asset Bundles ...");
    }

    public void AddFixedObjects()
    {
        Debug.Log("[ObjectManager] Adiciona objectos Fixos ...");    

        foreach (var fixedObj in _fixedObjects)
        {
            if (assetsObject.ContainsKey(fixedObj.name))
            {
                assetsObject[fixedObj.name] = fixedObj;

                Debug.Log("[ObjectManager]     Arquivo " + fixedObj.name + " sobrescrito por objeto do sistema.");
            }
            else
            {
                assetsObject.Add(fixedObj.name, fixedObj);
            }
        }

        Debug.Log("[ObjectManager] Objetos Fixos adicionados com sucesso!");
    }

    private void LoadAllObject()
    {
        Debug.Log($"[ObjectManager] Carrega todos os objetos!");

        ResetObjects();
        AddFixedObjects();
        LoadAssetBundles();
    }

    private void OnDownloadFinished(PhotonFileRequestReceiver instance)
    {
        Debug.Log($"[ObjectManager] Arquivo {instance.FileName} recebido com sucesso!");

        string filePath = Path.Combine(assetFolderPath, instance.FileName);

        File.WriteAllBytes(filePath, instance.bufferedFile.data);

        Debug.Log($"[ObjectManager]     Arquivo salvo em {filePath}");

        receivers.Remove(instance);
        instance.OnDownloadCompleted -= OnDownloadFinished;
        Destroy(instance.gameObject);

        if (receivers.Count != 0)
        {
            Debug.Log($"[ObjectManager]    Inicia proximo download ...");
            InstantiateReceiver(receivers[0]);
        }
        else
        {
            Debug.Log($"[ObjectManager]    Termina de baixar os objetos!");

            LoadAllObject();
            Debug.Log($"[ObjectManager]    ========================================");
            OnObjectDownloaded?.Invoke();

            progressView.SetActive(false);
        }
    }

    public void DownloadObjects()
    {
        Debug.Log($"[ObjectManager] Inicia download dos objetos:");

        // Update UI
        progressView.SetActive(true);

        // Delete all previous asset bundles
        Utils.DeleteFolderContent(assetFolderPath);

        // Add listener for Search files event
        NetworkPhoton.Instance.OnFileSearchResponse.AddListener(OnFileSearchResponse);

        // Send Event to request list of assets in the server
        EventManager.TriggerSendMessageRequest(Events.FILE_SEARCH_EVENT, new object[2] { Events.FILE_SEARCH_EVENT_CODE.REQUEST, FileRequestType.Obj });
    }

    private void InstantiateReceiver(PhotonFileRequestReceiver receiver)
    {
        receiver.Process();
        receiver.OnDownloadCompleted += OnDownloadFinished;

        txtDownloadCount.text = $"({totalObjectToDownload - receivers.Count + 1}/{totalObjectToDownload})";
    }

    private void OnFileSearchResponse(object[] message)
    {
        Debug.Log($"[ObjectManager] Recebe lista de arquivos do servidor!");

        NetworkPhoton.Instance.OnFileSearchResponse.RemoveListener(OnFileSearchResponse);

        string[] files = (string[])message[1];

        if (files.Length == 0)
        {
            Debug.Log("[ObjectManager] Nenhum arquivo recebido para download");

            LoadAllObject();
            OnObjectDownloaded?.Invoke();

            progressView.SetActive(false);
            return;
        }

        Debug.Log($"[ObjectManager]    Arquivos recebidos: " + files.Length);

        foreach (string file in files)
        {
            PhotonFileRequestReceiver receiver = Instantiate(receiverPrefab, requestParent).GetComponent<PhotonFileRequestReceiver>();
            receivers.Add(receiver);

            receiver.SetReceiverInfo(file, FileRequestType.Obj, OnDownloadProgressChange);
        }

        totalObjectToDownload = files.Length;

        Debug.Log($"[ObjectManager]    Inicia download individuais");

        InstantiateReceiver(receivers[0]);
    }

    private void OnDownloadProgressChange(float progress)
    {
        txtDownloadedFileName.text = $"{receivers[0].FileName} {progress}%";
        sliderProgress.value = progress;
    }

    public GameObject InstantiateObject(string name)
    {
        return InstantiateObject(name, Vector3.zero, Quaternion.identity, transform);
    }
    public GameObject InstantiateObject(string name, Transform parent)
    {
        return InstantiateObject(name, Vector3.zero, Quaternion.identity, parent);
    }

    public GameObject InstantiateObject(string name, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!assetsObject.ContainsKey(name)) return null;

        GameObject instance = Instantiate(assetsObject[name], position, rotation, parent);
        instance.name = name;

        return instance;
    }

    public string AssetsFolder { get => assetFolderPath; }
    public bool ContainsObject(string obj) => assetsObject.ContainsKey(obj);

    public List<GameObject> Objects { get => assetsObject.Values.ToList(); }
    public List<string> ObjectsName { get => assetsObject.Keys.ToList(); }
}