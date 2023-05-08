using Photon.Pun;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.UI;

public class ServerObjectManager : MonoBehaviour
{
    private Camera currentViewCamera;
    private Transform objectSelected;

    [SerializeField] private RawImage worldViewImage;

    [SerializeField] private float objectMovementSpeed;
    [SerializeField] private float objectRotationSpeed;

    [SerializeField] private Transform trackablesParent;

    private Vector3 anchor;
    private float xFactor;
    private float yFactor;
    private Ray ray;
    private RaycastHit hit;
    private Vector2Int currentResolution;
    private uint raycastRange = 1000;

    public LayerMask detectObjectMask;
    public LayerMask detectInteractablesObjectMask;
    public LayerMask envMask;

    private bool isCameraSelected = false;

    private object[] message = new object[6];
    private int objID = 0;

    private ObjectFile fileChoose;

    private void Awake()
    {
        if (currentViewCamera == null)
            currentViewCamera = Camera.main;

        objectSelected = transform;
    }

    private void Start()
    {
        UpdateScreenValues();
    }

    private void Update()
    {
        ray = currentViewCamera.ScreenPointToRay(new Vector2((Input.mousePosition.x - anchor.x) * xFactor, Input.mousePosition.y * yFactor));
        
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            EventManager.TriggerResetSelectedObject();
            PainelUIController.Instance.HideUI();
            AnimationUIController.Instance.HideUI();
        }

        if (Input.GetKeyUp(KeyCode.Delete) && objectSelected != null && !isCameraSelected)
        {
            message[0] = objectSelected.name;
            EventManager.TriggerSendMessageRequest(Events.DELETE_OBJECT, message);

            Destroy(objectSelected.gameObject);

            EventManager.TriggerResetSelectedObject();
            PainelUIController.Instance.HideUI();
            AnimationUIController.Instance.HideUI();
        }

        if (Input.GetMouseButtonUp(0) && Input.mousePosition.x > anchor.x)
        {
            if (CheckIfScreenSizeChange())
                UpdateScreenValues();
        }

        Debug.DrawRay(ray.origin, ray.direction * raycastRange, Color.yellow);

        if (isCameraSelected)
            HandleUserInput();
    }

    float zCoord;

    private void HandleUserInput()
    {
        float upDown = Input.GetAxis("UpDown");
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(upDown) + Mathf.Abs(vertical) + Mathf.Abs(horizontal) >= 0.01f) 
            objectSelected.Translate(new Vector3(horizontal, upDown, vertical) * objectMovementSpeed * Time.deltaTime);
        //objectSelected.localPosition += new Vector3(horizontal, upDown, vertical) * objectMovementSpeed * Time.deltaTime;

        //objectSelected.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y) * Time.deltaTime * 10);
        objectSelected.localEulerAngles += (new Vector3(Input.GetAxis("RotateX"), Input.GetAxis("RotateY"), 0) * objectRotationSpeed * Time.deltaTime);
    }

    private bool CheckIfScreenSizeChange()
    {
        return currentResolution.x != Screen.width || currentResolution.y != Screen.height ;
    }

    private void UpdateScreenValues()
    {
        currentResolution.x = Screen.width;
        currentResolution.y = Screen.height;

        SetAnchors(currentResolution.x, currentResolution.y);
        EventManager.TriggerScreenSizeChange(currentResolution.x, currentResolution.y);
    }

    private Vector3 GetWorldMousePos()
    {
        Vector3 mousePosition = new Vector3((Input.mousePosition.x - anchor.x) * xFactor, Input.mousePosition.y * yFactor, zCoord);

        return currentViewCamera.ScreenToWorldPoint(mousePosition);
    }

    
    private void OnObjectSelected(Transform selectedObject)
    {
        Debug.Log($"Objeto selecionado = {selectedObject.name}");

        objectSelected = selectedObject;
        isCameraSelected = selectedObject.gameObject.layer == LayerMask.NameToLayer("Camera");
        Debug.Log(isCameraSelected);

        InteractableObject interactableObject = selectedObject.GetComponent<InteractableObject>();

        if (interactableObject == null) return;

        switch (interactableObject.Type)
        {
            case InteractableObject.ObjectType.Painel:
                PainelController painel = interactableObject.GetComponent<PainelController>();

                if (painel.PainelFileType == ExtensionAllowed.ExtensionValue.Video)
                {
                    PainelUIController.Instance.SetPainel(painel);
                }
                else
                {
                    PainelUIController.Instance.HideUI();
                }

                AnimationUIController.Instance.HideUI();
                break;

            case InteractableObject.ObjectType.Animation:
                AnimationUIController.Instance.SetAnimation(interactableObject.GetComponent<AnimationController>());
                PainelUIController.Instance.HideUI();

                break;

            default:
                AnimationUIController.Instance.HideUI();
                PainelUIController.Instance.HideUI();
                break;
        }
    }

    private void SetAnchors(int width, int height)
    {
        // Calcula dimensoes da imagem Camera View. Usado para identificar a posicao que o mouse esta apontando na imagem.
        Vector3[] corners = new Vector3[4];
        worldViewImage.rectTransform.GetWorldCorners(corners);
        anchor = new Vector2(corners[0].x, corners[1].y);
        xFactor = width / (width - anchor.x);
        yFactor = height / (height - anchor.y);

        UpdateTranformGizmo();
    }

    private void OnCameraViewChange(Camera camera)
    {
        currentViewCamera = camera;
        UpdateTranformGizmo();
    }

    private void UpdateTranformGizmo()
    {
        TransformGizmo tg = currentViewCamera.GetComponent<TransformGizmo>();

        if (tg != null)
        {
            tg.SetCameraArgs(anchor, xFactor, yFactor);
        }
    }

    private void OnEnable()
    {
        EventManager.OnObjectSelected += OnObjectSelected;
        EventManager.OnCameraViewChange += OnCameraViewChange;
    }

    private void OnDisable()
    {
        EventManager.OnObjectSelected -= OnObjectSelected;
        EventManager.OnCameraViewChange -= OnCameraViewChange;
    }

    public void OnPrefabChoose(string fileName)
    {
        fileChoose = new ObjectFile(fileName);
    }

    public void CreateObject()
    {
        Debug.Log("Objeto a ser criado: " + fileChoose.FileName);

        string objectoToCreate;
        GameObject currentInstance;

        if (fileChoose.ExtensionValue == ExtensionAllowed.ExtensionValue.Prefab)
        {
            currentInstance = Instantiate(Resources.Load<GameObject>($"Prefabs/{fileChoose.Name}"));
            objectoToCreate = fileChoose.Name;
        }
        else
        {
            currentInstance = Instantiate(Resources.Load<GameObject>($"Prefabs/Painel"));
            currentInstance.GetComponent<PainelController>().LoadFile(fileChoose);
            objectoToCreate = "Painel";
        }

        currentInstance.name = $"obj{objID++}";
        currentInstance.transform.rotation = trackablesParent.rotation;
        currentInstance.transform.position = currentViewCamera.transform.position + currentViewCamera.transform.forward * 0.7f;
        currentInstance.transform.parent = trackablesParent;

        currentInstance.GetComponent<InteractableObject>().SetActive(true);

        message[0] = ObjectType.Interactable;
        message[1] = objectoToCreate;
        message[2] = currentInstance.name;
        message[3] = currentInstance.transform.localPosition;
        message[4] = currentInstance.transform.localEulerAngles;
        message[5] = fileChoose.FileName;

        EventManager.TriggerSendMessageRequest(Events.CREATE_OBJECT, message);
    }
    
}
