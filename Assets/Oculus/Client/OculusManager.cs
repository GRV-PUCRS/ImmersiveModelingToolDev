using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class OculusManager : Singleton<OculusManager>
{
    [Header("Referencias")]
    [SerializeField] private Transform worldAnchor;
    [SerializeField] private OVRPassthroughLayer oVRPassthroughLayer;
    [SerializeField] private GameObject defaultVRScenario;
    [SerializeField] private GameObject logger;
    [SerializeField] private OcclusionObject referenceModel;
    [SerializeField] private Transform initialScreen;
    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject library;

    [Header("Objectos com Oclusao")]
    [SerializeField] private OcclusionObject environmentObject;
    [SerializeField] private OcclusionObject interactablesObjects;

    private object[] message = new object[4];

    public Transform leftAxis;
    public Transform rightAxis;

    private bool scaleX = true, scaleY = true, scaleZ = true;
    private bool isEditMode = false;
    private bool isOcclusionObjVisible = false;
    private bool toReset = false;
    private bool firstAnchor = true;

    public TaskManager taskManager;

    public static event Action<bool> OnEditModeChange;

    private List<OcclusionObject> occlusionObjects;

    private void Awake()
    {
        //rightAxis.localEulerAngles = new Vector3(311.8f, 180, 172);

        initialScreen.gameObject.SetActive(true);
        environment.SetActive(false);
        occlusionObjects = new List<OcclusionObject>();
    }

    private void Start()
    {
        ToggleARVRMode(true);
        ToggleDevMode(false);
        ToggleEditMode(true);

        OnStartButtonClicked();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            ToggleEditMode(!isEditMode);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            ToggleDevMode(!logger.activeInHierarchy);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            OnRightHandTriggerClicked();
        }
    }

    private void OnStartButtonClicked()
    {
        toReset = !toReset;

        leftAxis.gameObject.SetActive(toReset);
        rightAxis.gameObject.SetActive(toReset);
    }

    private void OnRightHandTriggerClicked()
    {
        if (!toReset) return;

        worldAnchor.position = rightAxis.position;
        //worldAnchor.rotation = Quaternion.Euler(new Vector3(0, controllerRightTransform.rotation.eulerAngles.y, 0));
        worldAnchor.rotation = rightAxis.rotation;
        //worldAnchor.position += worldAnchor.right * .07f;

        message[0] = ObjectType.Flag;
        message[1] = "ENV_TAG_1";
        message[2] = worldAnchor.position;
        message[3] = worldAnchor.rotation.eulerAngles;

        EventManager.TriggerSendMessageRequest(Events.UPDATE_TAG, message);

        if (firstAnchor)
        {
            OnStartButtonClicked();
            initialScreen.gameObject.SetActive(false);
            environment.SetActive(true);
            firstAnchor = false;
        }
    }

    private void OnButtonOneClicked()
    {
        environmentObject.ToggleVisibility();
        EventManager.TriggerSendMessageRequest(environmentObject.Visibility ? Events.SHOW_OBJECT : Events.HIDE_OBJECT, new object[1] { "env" });
    }

    private void OnButtonTwoClicked()
    {
        /*
        interactablesObjects.ToggleVisibility();
        EventManager.TriggerSendMessageRequest(interactablesObjects.Visibility ? Events.SHOW_OBJECT : Events.HIDE_OBJECT, new object[1] { "obj" });
        */

        ToggleOcclusionObjects(!isOcclusionObjVisible);
    }

    public void ToggleARVRMode(bool newValue)
    {
        oVRPassthroughLayer.hidden = newValue;
        defaultVRScenario.SetActive(newValue);

        if (newValue)
        {
            //referenceModel.SetObjectVisibility(true);
            //environmentObject.SetObjectVisibility(true);

            //referenceModel.gameObject.SetActive(false);
            //environmentObject.gameObject.SetActive(false);

            ToggleOcclusionObjects(true);

            InputController.Instance.OnOneButtonPressed.RemoveListener(OnButtonOneClicked);
            InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
        }
        else
        {
            //referenceModel.gameObject.SetActive(true);
            //environmentObject.gameObject.SetActive(true);

            ToggleOcclusionObjects(isOcclusionObjVisible);

            InputController.Instance.OnOneButtonPressed.AddListener(OnButtonOneClicked);
            InputController.Instance.OnTwoButtonPressed.AddListener(OnButtonTwoClicked);
        }
    }

    public void ToggleDevMode(bool newValue)
    {
        logger.SetActive(newValue);

        if (newValue)
        {
            InputController.Instance.OnThreeButtonPressed.AddListener(ResetTaskManager);
        }
        else
        {
            InputController.Instance.OnThreeButtonPressed.RemoveListener(ResetTaskManager);
        }
    }

    public void SetObjectAsOrigin(DragUI referenceObject)
    {
        if (referenceObject.IsReferenceObject)
        {
            taskManager.RmvFixedObject(referenceObject);
            SoundManager.Instance.PlaySound(SoundManager.Instance.resetOrigin);

            Debug.Log("------> Objeto removido como fixo" + referenceObject.name + " " + referenceObject.TransformToUpdate.name);
        }
        else
        {
            referenceObject.TransformToUpdate.localPosition = Vector3.zero;
            referenceObject.TransformToUpdate.localRotation = Quaternion.identity;

            taskManager.AddFixedObject(referenceObject);
            SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);

            Debug.Log("------> Objeto adicionado como fixo" + referenceObject.name + " " + referenceObject.TransformToUpdate.name);
        }

        referenceObject.IsReferenceObject = !referenceObject.IsReferenceObject;
    }

    public void SetFixedObject(DragUI referenceObject)
    {
        taskManager.AddObjectToAllStages(referenceObject.TransformToUpdate.gameObject);
        SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);
    }


    public void AddObjectToTask(Transform obj)
    {
        taskManager.AddObjectInTask(obj);
    }

    public void ResetSession()
    {
        occlusionObjects.Clear();
    }

    public void ToggleEditMode(bool newValue)
    {
        //library.SetActive(newValue);
        isEditMode = newValue;

        OnEditModeChange?.Invoke(newValue);

        if (!isEditMode)
            ToggleOcclusionObjects(false);
    }

    public void SetOcclusionObject(DragUI dragUI)
    {
        OcclusionObject occlusionObject = dragUI.GetComponent<OcclusionObject>();

        if (occlusionObject == null)
        {
            occlusionObject = dragUI.gameObject.AddComponent<OcclusionObject>();
            occlusionObject.SetObjectVisibility(defaultVRScenario.activeInHierarchy ? true : isOcclusionObjVisible);
            occlusionObjects.Add(occlusionObject);
            taskManager.AddFixedObject(dragUI);
        }
        else
        {
            occlusionObject.SetObjectVisibility(true);
            occlusionObjects.Remove(occlusionObject);
            taskManager.RmvFixedObject(dragUI);
            Destroy(occlusionObject);
        }
    }

    public void ToggleOcclusionObjects(bool newValue)
    {
        if (defaultVRScenario.activeInHierarchy && !newValue) return;

        isOcclusionObjVisible = newValue;

        occlusionObjects.ForEach(e => e.SetObjectVisibility(isOcclusionObjVisible));

        EventManager.TriggerToggleObjectVisibility(isOcclusionObjVisible);
    }

    public void ResetTaskManager()
    {
        //taskManager.ResetTasks();
    }

    public void SetScaleX(bool newValue)
    {
        scaleX = newValue;
    }
    public void SetScaleY(bool newValue)
    {
        scaleY = newValue;
    }
    public void SetScaleZ(bool newValue)
    {
        scaleZ = newValue;
    }

    public bool ScaleX { get => scaleX; }
    public bool ScaleY { get => scaleY; }
    public bool ScaleZ { get => scaleZ; }
    public bool IsEditMode { get => isEditMode; }
    public bool IsOcclusionObjVisible { get => isOcclusionObjVisible; }

    private void OnEnable()
    {
        InputController.Instance.OnStartButtonPressed.AddListener(OnStartButtonClicked);
        InputController.Instance.OnOneButtonPressed.AddListener(OnButtonOneClicked);
        InputController.Instance.OnTwoButtonPressed.AddListener(OnButtonTwoClicked);
        InputController.Instance.OnRightHandTriggerUp.AddListener(OnRightHandTriggerClicked);
    }

    private void OnDisable()
    {
        InputController.Instance.OnStartButtonPressed.RemoveListener(OnStartButtonClicked);
        InputController.Instance.OnOneButtonPressed.RemoveListener(OnButtonOneClicked);
        InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
        InputController.Instance.OnRightHandTriggerUp.RemoveListener(OnRightHandTriggerClicked);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
