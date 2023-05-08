using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectStore : Singleton<ObjectStore>
{
    [Header("References")]
    [SerializeField] private Collider objectsContainerRenderer;
    [SerializeField] private Transform objectsContainer;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private Transform referencesPositions;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip createObjectSoundEffect;
    [SerializeField] private AudioClip deleteObjectSoundEffect;

    [Header("Library Configurations")]
    [SerializeField, Range(1, 6)] private int cols;
    [SerializeField, Range(1, 6)] private int rows;
    [SerializeField, Range(0.1f, 1)] private float maxSizeScale = 1f;

    [Header("View")]
    [SerializeField] private MeshRenderer meshView;
    [SerializeField] private BoxCollider viewCollider;

    [Header("UI References")]
    [SerializeField] private GameObject refreshObjsBTN;
    [SerializeField] private GameObject loadscreenIMG;
    [SerializeField] private GameObject btnNextPage;
    [SerializeField] private GameObject btnPrevPage;
    [SerializeField] private GameObject btnReset;

    private bool toDelete = false;
    private bool toCreate = false;
    private Transform currentObj = null;
    private Dictionary<string, Vector3> fixedPositions;
    private Dictionary<string, Vector3> fixedRotations;

    private float offsetX;
    private float offsetY;

    private List<GameObject> objs = new List<GameObject>();
    private List<Transform> fixedReferencesPositions;
    private AudioSource audioSource;

    private int currentPage = 0;
    private int pageAmount = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        fixedReferencesPositions = new List<Transform>(referencesPositions.GetComponentsInChildren<Transform>());
        fixedReferencesPositions.Remove(referencesPositions);
    }

    private void Start()
    {
        //LoadObject();
        //UpdatePage();

        refreshObjsBTN.SetActive(true);
        loadscreenIMG.SetActive(false);

        OnEditModeChange(OculusManager.Instance.IsEditMode);

        //DownloadObjects();
        LoadObject();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            PreviousPage();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            //NextPage();
            //Temp();
            LoadObject();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            //LoadAssetsBundles();

            LoadObject();
            UpdatePage();
        }
    }

    public void DownloadObjects()
    {
        SetActiveInterface(false);
        SetActiveLoadScreen(true);

        Utils.ClearChilds(objectsContainer);

        ObjectManager.Instance.DownloadObjects();
    }

    public void ResetObjects()
    {
        Utils.ClearChilds(objectsContainer);

        ObjectManager.Instance.ResetObjects();
        ObjectManager.Instance.AddFixedObjects();

        LoadObject();
    }

    private void SetActiveInterface(bool value)
    {
        refreshObjsBTN.SetActive(value);
        btnNextPage.SetActive(value);
        btnPrevPage.SetActive(value);
        btnReset.SetActive(value);
    }

    private void SetActiveLoadScreen(bool value)
    {
        loadscreenIMG.SetActive(value);
    }
    /*
    private List<DragUI> LoadAssetsBundles()
    {
        List<DragUI> assetBundles = new List<DragUI>();

        if (!Directory.Exists(Application.persistentDataPath + "/bundles")) return assetBundles;

        Utils.ClearChilds(objectsContainer);

        foreach (string file in Directory.GetFiles(Application.persistentDataPath + "/bundles"))
        {
            Debug.Log(">>>>> " + file);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
            
            if (assetBundle != null)
            {
                string[] substring = file.Replace("\\", "/").Split('/');
                string assetName = substring[substring.Length - 1];

                GameObject gameObject = Instantiate(assetBundle.LoadAsset<GameObject>(assetName), objectsContainer);
                gameObject.name = assetName;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.GetComponentInChildren<DragUI>().SetNewTransform(gameObject.transform);

                //gameObject.transform.position = Vector3.zero;
                //gameObject.transform.rotation = Quaternion.identity;

                Debug.Log("Objeto " + assetName + " instanciado !");

                assetBundles.Add(gameObject.GetComponent<DragUI>());
            }
        }

        return assetBundles;
    }
    */

    private void LoadObject()
    {
        Logger.Log("[LoadObject] Carrega Objetos: ");

        objs.ForEach(obj => Destroy(obj));
        objs.Clear();

        fixedPositions = new Dictionary<string, Vector3>();
        fixedRotations = new Dictionary<string, Vector3>();

        offsetX = objectsContainerRenderer.bounds.size.x * 0.8f / cols;
        offsetY = objectsContainerRenderer.bounds.size.y * 0.8f / rows;

        int counter = 0;
        Vector3 maxSize = new Vector3(offsetX * maxSizeScale, offsetY * maxSizeScale, objectsContainerRenderer.bounds.size.z / 2);

        Debug.Log("---->" + ObjectManager.Instance.ObjectsName.Count);

        //foreach (DragUI dragUI in objectsContainer.GetComponentsInChildren<DragUI>())
        foreach (string objName in ObjectManager.Instance.ObjectsName)
        {
            GameObject instance = ObjectManager.Instance.InstantiateObject(objName, objectsContainer);

            if (instance == null)
            {
                Logger.Log("[LoadObject][ERRO] Erro ao instanciar o objeto " + objName);
                continue;
            }

            Logger.Log("[ObjectStore]   Carrega "+ objName);
            instance.name = objName;

            DragUI dragUI = instance.GetComponent<DragUI>();
            dragUI.SetNewTransform(dragUI.transform);

            Utils.SetLayer(dragUI.TransformToUpdate, "StoredObject", false);
            /*
            dragUI.TransformToUpdate.gameObject.AddComponent<Outline>();
            dragUI.TransformToUpdate.gameObject.GetComponent<Outline>().enabled = false;
            */
            Bounds bounds = CalculateBounds(dragUI.gameObject);

            dragUI.TransformToUpdate.localPosition = fixedReferencesPositions[counter].localPosition;

            Vector3 oldScale = dragUI.TransformToUpdate.transform.localScale;
            Vector3 newScale = new Vector3();

            newScale.x = bounds.size.x > maxSize.x ? oldScale.x * maxSize.x / bounds.size.x : oldScale.x;
            newScale.y = bounds.size.y > maxSize.y ? oldScale.y * maxSize.y / bounds.size.y : oldScale.y;
            newScale.z = bounds.size.z > maxSize.z ? oldScale.z * maxSize.z / bounds.size.z : oldScale.z;

            newScale.x = Mathf.Min(newScale.x, newScale.y, newScale.z);
            newScale.y = newScale.z = newScale.x;

            dragUI.TransformToUpdate.localScale = newScale;

            fixedPositions.Add(dragUI.name, fixedReferencesPositions[counter].localPosition);
            fixedRotations.Add(dragUI.name, fixedReferencesPositions[counter].localEulerAngles);
            objs.Add(instance);

            counter = (counter + 1) % (rows * cols);

            Logger.Log("[ObjectStore]   " + objName + " carregado com sucesso!");
        }
        
        pageAmount = objs.Count / (rows * cols);

        SetActiveLoadScreen(false);
        SetActiveInterface(true);

        currentPage = 0;
        UpdatePage();

        Logger.Log("[LoadObject] Objetos carregados com sucesso!");
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        
        foreach (MeshRenderer mr in obj.GetComponentsInChildren<MeshRenderer>())
        {
            bounds.Encapsulate(mr.bounds);
        }
        
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
        {
            bounds.Encapsulate(c.bounds);
        }

        return bounds;
    }

    private void UpdatePage()
    {
        int beginIndex = currentPage * (rows * cols);
        int endIndex = Mathf.Min(beginIndex + (rows * cols)-1, objs.Count - 1);

        Debug.Log("OBJS " + objs.Count);

        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].SetActive(i >= beginIndex && i <= endIndex);
            objs[i].transform.localPosition = fixedPositions[objs[i].name];
            objs[i].transform.localEulerAngles = fixedRotations[objs[i].name];
        }
    }

    public void PreviousPage()
    {
        currentPage = Mathf.Max(currentPage - 1, 0);

        UpdatePage();
    }

    public void NextPage()
    {
        currentPage = Mathf.Min(currentPage + 1, pageAmount);

        UpdatePage();
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObj == null) return;

        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("StoredObject")))
        {
            toCreate = true;
            toDelete = false;
        }
        else
        {
            toCreate = toDelete = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentObj == null) return;

        toDelete = true;
        toCreate = false;
    }

    private void OnObjectDragBegin(Transform obj)
    {
        Logger.Log("[ObjectStore] Drag Begin: " + obj.name);
        currentObj = obj;
    }

    private void OnObjectDragEnd(Transform obj)
    {
        Debug.Log($"=> {toDelete} {toCreate} {obj}");

        if (toDelete && obj != objectsContainerRenderer.transform)
        {
            //Destroy(currentObj.gameObject);
            if (objs.Contains(obj.gameObject))
            {
                obj.localPosition = fixedPositions[obj.name];
                obj.localEulerAngles = fixedRotations[obj.name];
            }
            else
            {
                Destroy(obj.gameObject);
                PlayAudio(deleteObjectSoundEffect);
            }
        }
        else if (toCreate)
        {
            Debug.Log("Create Object " + obj.name);
            if (fixedPositions.ContainsKey(obj.name) && obj.GetComponent<IAction>() == null)
            {
                /*
                obj.GetComponentInChildren<Outline>().enabled = false;
                obj.GetComponentInChildren<Outline>().DisableOutline();
                */

                // Cria objeto na posicao que o evento foi disparado
                Transform instance = ObjectManager.Instance.InstantiateObject(obj.name).transform;
                instance.name = obj.name;
                instance.position = obj.position;
                instance.rotation = obj.rotation;
                instance.localScale = Vector3.one;
                Utils.SetLayer(instance, "InteractableObject", false);
                taskManager.AddObjectInTask(instance);

                Debug.Log($"Objecto {obj.name} criado com sucesso!");

                // Reposiciona objeto na preteleira
                obj.localPosition = fixedPositions[obj.name];
                obj.localEulerAngles = fixedRotations[obj.name];

                PlayAudio(createObjectSoundEffect);

                InteractableObject io = instance.GetComponent<InteractableObject>();

                if (io.Type == InteractableObject.ObjectType.Painel)
                {
                    if (io.CompareTag("TextPainel"))
                    {
                        io.GetComponent<PainelController>().SetTextPainel("Texto");
                        instance.name = "Texto.ptx";
                    }
                }
                else
                {
                    /*
                    Outline outline = instance.gameObject.GetComponent<Outline>();

                    if (outline == null)
                        instance.gameObject.AddComponent<Outline>();

                    instance.gameObject.GetComponent<Outline>().enabled = false;*/
                }

                Debug.Log("Object criado!");
            }
        }
        
        toDelete = toCreate = false;
        taskManager.SaveCurrentStage();
        taskManager.SavePlan();

        currentObj = null;
    }

    public void RetrieveObjectToStore(Transform obj)
    {
        if (!objs.Contains(obj.gameObject)) return;

        obj.localPosition = fixedPositions[obj.name];
        obj.localEulerAngles = fixedRotations[obj.name];

        UpdatePage();
    }

    public void PlayAudio(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.Play();
    }

    private void OnEditModeChange(bool newValue)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(newValue);
        }

        meshView.enabled = newValue;
        viewCollider.enabled = newValue;
    }

    private void OnEnable()
    {
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        OculusManager.OnEditModeChange += OnEditModeChange;

        ObjectManager.Instance.OnObjectDownloaded += LoadObject;
    }

    private void OnDisable()
    {
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        OculusManager.OnEditModeChange -= OnEditModeChange;

        ObjectManager.Instance.OnObjectDownloaded -= LoadObject;
    }
}
