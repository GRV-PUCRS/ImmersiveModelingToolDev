using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ObjectStore : Singleton<ObjectStore>
{
    [Header("References")]
    [SerializeField] private Transform baseLibrary;
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
    [SerializeField] private GameObject deleteObjectView;
    [SerializeField] private Toggle _toggleAll;
    [SerializeField] private Toggle _toggleActions;
    [SerializeField] private Toggle _toggleObjects;

    private bool toDelete = false;
    private bool toCreate = false;
    private Transform currentObj = null;

    private float offsetX;
    private float offsetY;

    private List<StoredObject> allObjects;
    private List<StoredObject> activatedObjects;
    private List<Transform> fixedReferencesPositions;

    private int currentPage = 0;
    private int pageAmount = 0;

    private void Awake()
    {
        allObjects = new List<StoredObject>();
        activatedObjects = new List<StoredObject>();

        fixedReferencesPositions = new List<Transform>(referencesPositions.GetComponentsInChildren<Transform>());
        fixedReferencesPositions.Remove(referencesPositions);
    }

    private void Start()
    {
        //LoadObject();
        //UpdatePage();

        refreshObjsBTN.SetActive(true);
        loadscreenIMG.SetActive(false);
        deleteObjectView.SetActive(false);

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
    }

    public void DownloadObjects()
    {
        SetActiveInterface(false);
        SetButtonsVisibility(false);
        SetActiveLoadScreen(true);

        Utils.ClearChilds(objectsContainer);

        ObjectManager.Instance.DownloadObjects();
    }

    public void ResetObjects()
    {
        Utils.ClearChilds(objectsContainer);

        ObjectManager.Instance.DeleteObjects();
        ObjectManager.Instance.ResetObjects();
        ObjectManager.Instance.AddFixedObjects();

        LoadObject();
    }

    private void SetActiveInterface(bool value)
    {
        btnNextPage.SetActive(value);
        btnPrevPage.SetActive(value);

        SetButtonsVisibility(!SessionManager.Instance.IsGuess);
    }

    private void SetActiveLoadScreen(bool value)
    {
        loadscreenIMG.SetActive(value);
    }

    private void LoadObject()
    {
        Debug.Log("[LoadObject] Carrega Objetos: ");

        allObjects.ForEach(obj => Destroy(obj.Instance));
        allObjects.Clear();
        activatedObjects.Clear();

        offsetX = objectsContainerRenderer.bounds.size.x * 0.8f / cols;
        offsetY = objectsContainerRenderer.bounds.size.y * 0.8f / rows;

        int counter = 0;
        Vector3 maxSize = new Vector3(offsetX * maxSizeScale, offsetY * maxSizeScale, objectsContainerRenderer.bounds.size.z / 2);

        foreach (string objName in ObjectManager.Instance.ObjectsName)
        {
            GameObject instance = ObjectManager.Instance.InstantiateObject(objName, objectsContainer);

            if (instance == null)
            {
                Debug.Log("[LoadObject][ERRO] Erro ao instanciar o objeto " + objName);
                continue;
            }

            Debug.Log("[ObjectStore]   Carrega " + objName);
            instance.name = objName;

            DragUI dragUI = instance.GetComponentInChildren<DragUI>();
            dragUI.SetNewTransform(dragUI.transform);

            Utils.SetLayer(dragUI.TransformToUpdate, "StoredObject", false);

            dragUI.TransformToUpdate.gameObject.AddComponent<Outline>();
            //dragUI.TransformToUpdate.gameObject.GetComponent<Outline>().enabled = false;

            Bounds bounds = CalculateBounds(dragUI.gameObject);

            Vector3 oldScale = dragUI.TransformToUpdate.transform.localScale;
            Vector3 newScale = new Vector3();

            newScale.x = bounds.size.x > maxSize.x ? oldScale.x * maxSize.x / bounds.size.x : oldScale.x;
            newScale.y = bounds.size.y > maxSize.y ? oldScale.y * maxSize.y / bounds.size.y : oldScale.y;
            newScale.z = bounds.size.z > maxSize.z ? oldScale.z * maxSize.z / bounds.size.z : oldScale.z;

            newScale.x = Mathf.Min(newScale.x, newScale.y, newScale.z);
            newScale.y = newScale.z = newScale.x;

            dragUI.TransformToUpdate.localScale = newScale;

            allObjects.Add(new StoredObject(instance, instance.HasComponent<AbstractAction>(), Vector3.zero, Quaternion.identity));

            counter = (counter + 1) % (rows * cols);

            Debug.Log("[ObjectStore]   " + objName + " carregado com sucesso!");
        }

        SetActiveLoadScreen(false);
        SetActiveInterface(true);

        OnToggleSelected();

        Debug.Log("[LoadObject] Objetos carregados com sucesso!");
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

    private void UpdateStoredObjects()
    {
        allObjects.ForEach(so => so.SetActive(false));

        if (_toggleAll.isOn)
        {
            UpdateStoredObjects(allObjects);
        }
        else
        {
            if (_toggleActions.isOn)
            {
                UpdateStoredObjects(allObjects.Where(obj => obj.IsAction).ToList());
            }
            else
            {
                UpdateStoredObjects(allObjects.Where(obj => !obj.IsAction).ToList());
            }
        }
    }

    private void UpdateStoredObjects(List<StoredObject> storedObjects)
    {
        int i = 0;
        foreach (var obj in storedObjects)
        {
            obj.LocalPosition = fixedReferencesPositions[i].localPosition;
            obj.LocalRotation = fixedReferencesPositions[i].localRotation;

            obj.ResetLocation();

            i = (i + 1) % fixedReferencesPositions.Count;
        }

        activatedObjects = storedObjects;
        pageAmount = storedObjects.Count / (rows * cols);
    }

    public void OnToggleSelected()
    {
        currentPage = 0;
        UpdateStoredObjects();
        UpdatePage();
    }

    private void UpdatePage()
    {
        int beginIndex = currentPage * (rows * cols);
        int endIndex = Mathf.Min(beginIndex + (rows * cols) - 1, activatedObjects.Count - 1);

        for (int i = 0; i < activatedObjects.Count; i++)
        {
            activatedObjects[i].SetActive(i >= beginIndex && i <= endIndex);
            //objs[i].transform.localPosition = fixedPositions[objs[i].name];
            //objs[i].transform.localEulerAngles = fixedRotations[objs[i].name];
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
        
        deleteObjectView.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentObj == null) return;

        toDelete = true;
        toCreate = false;

        if (Utils.IsInteractableObject(other.gameObject))
            deleteObjectView.SetActive(true);
    }

    private void OnObjectDragBegin(DragUI obj)
    {
        if (obj.TransformToUpdate == baseLibrary) return;

        currentObj = obj.transform;
    }

    private void DeleteObject(DragUI obj)
    {
        //Destroy(currentObj.gameObject);
        var storedObject = allObjects.Find(element => obj.gameObject == element.Instance);

        if (storedObject != null)
        {
            storedObject.ResetLocation();
        }
        else
        {
            if (OculusManager.Instance.SelectionList.Count != 0)
            {
                GameObject[] selection = new GameObject[OculusManager.Instance.SelectionList.Count];
                OculusManager.Instance.SelectionList.CopyTo(selection);
                OculusManager.Instance.ClearSelection();

                foreach (GameObject sceneElement in selection)
                {
                    Destroy(sceneElement);
                }
            }
            else
            {
                Destroy(obj.TransformToUpdate.gameObject);
            }

            SoundManager.Instance.PlaySound(deleteObjectSoundEffect);

            deleteObjectView.SetActive(false);
        }
    }

    private void CreateObject(DragUI obj)
    {
        Debug.Log("Create Object " + obj.name);

        //obj.GetComponentInChildren<Outline>().enabled = false;
        obj.GetComponentInChildren<Outline>().DisableOutline();


        // Cria objeto na posicao que o evento foi disparado
        Transform sceneElementInstance = new GameObject("SceneElement").transform;
        taskManager.AddObjectInTask(sceneElementInstance);

        sceneElementInstance.localPosition = Vector3.zero;
        sceneElementInstance.localEulerAngles = Vector3.zero;

        Transform instance = ObjectManager.Instance.InstantiateObject(obj.name).transform;
        instance.gameObject.SetActive(true);

        instance.name = obj.name;
        instance.position = obj.transform.position;
        instance.rotation = obj.transform.rotation;
        instance.localScale = Vector3.one;
        instance.SetParent(sceneElementInstance);
        instance.GetComponentInChildren<DragUI>().SetNewTransform(sceneElementInstance);
        instance.gameObject.AddComponent<Outline>();

        Utils.SetLayer(instance, "InteractableObject", false);

        Debug.Log($"Objecto {obj.name} criado com sucesso!");

        // Reposiciona objeto na preteleira
        var storedObject = allObjects.Find(so => so.Instance == obj.gameObject);
        storedObject.ResetLocation();

        SoundManager.Instance.PlaySound(createObjectSoundEffect);

        ITrackebleObject trackableObj = instance.GetComponentInChildren<ITrackebleObject>();
        if (trackableObj == null)
        {
            instance.gameObject.AddComponent<InteractableObjectTrackable>();
        }

        IInteractor interactor = instance.GetComponent<IInteractor>();
        if (interactor != null) interactor.OnInstantiated();

        if (instance.TryGetComponent<ModelSizeHandler>(out var modeSizeHandler))
        {
            instance.localScale = modeSizeHandler.GetDesirableSize();
        }
    }

    private void ActiveAction(IAction action)
    {
        action.OnInstantiated();
    }

    private void OnObjectDragEnd(DragUI obj)
    {
        if (toDelete)
        {
            DeleteObject(obj);
        }
        else if (toCreate)
        {
            IAction action = obj.GetComponent<IAction>();

            if (action != null)
            {
                ActiveAction(action);
            }
            else
            {
                CreateObject(obj);
            }
        }

        toDelete = toCreate = false;
        currentObj = null;
    }

    public void RetrieveObjectToStore(Transform obj)
    {
        var storedObject = allObjects.Find(so => so.Instance == obj.gameObject);
        if (storedObject == null) return;

        storedObject.ResetLocation();

        UpdatePage();
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

    public void SetButtonsVisibility(bool newValue)
    {
        Debug.Log("BTN " + newValue);
        refreshObjsBTN.SetActive(newValue);
    }

    private void OnEnable()
    {
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        EventManager.OnExecModeChange += OnEditModeChange;

        ObjectManager.Instance.OnObjectDownloaded += LoadObject;
    }

    private void OnDisable()
    {
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        EventManager.OnExecModeChange -= OnEditModeChange;

        ObjectManager.Instance.OnObjectDownloaded -= LoadObject;
    }

    private class StoredObject
    {
        public GameObject Instance { get; }
        public bool IsAction { get; }
        public Vector3 LocalPosition { get; set; }
        public Quaternion LocalRotation { get; set; }

        public StoredObject(GameObject instance, bool isAction, Vector3 localPosition, Quaternion localRotation)
        {
            Instance = instance;
            IsAction = isAction;
            LocalPosition = localPosition;
            LocalRotation = localRotation;
        }

        public void SetActive(bool value) => Instance.SetActive(value);
        public void ResetLocation()
        {
            Instance.transform.localPosition = LocalPosition;
            Instance.transform.localRotation = LocalRotation;
        }
    }
}