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

        if (Input.GetKeyUp(KeyCode.B))
        {
            OnButtonTwoClicked();
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

        taskManager.SetTaskManagerVisibility(!toReset);
    }

    private void OnRightHandTriggerClicked()
    {
        if (!toReset) return;

        worldAnchor.position = rightAxis.position;
        worldAnchor.rotation = rightAxis.rotation;

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
    }

    private void OnButtonTwoClicked()
    {
        ToggleOcclusionObjects(!isOcclusionObjVisible);
    }

    public void ToggleARVRMode(bool newValue)
    {
        oVRPassthroughLayer.hidden = newValue;
        defaultVRScenario.SetActive(newValue);

        if (newValue)
        {
            ToggleOcclusionObjects(true);

            InputController.Instance.OnOneButtonPressed.RemoveListener(OnButtonOneClicked);
            InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
        }
        else
        {
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
            taskManager.RmvPersistentObject(referenceObject);
            SoundManager.Instance.PlaySound(SoundManager.Instance.resetOrigin);

            Debug.Log("------> Objeto removido como fixo" + referenceObject.name + " " + referenceObject.TransformToUpdate.name);
        }
        else
        {
            referenceObject.TransformToUpdate.localPosition = Vector3.zero;
            referenceObject.TransformToUpdate.localRotation = Quaternion.identity;

            taskManager.AddPersistentObject(referenceObject);
            SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);

            Debug.Log("------> Objeto adicionado como fixo" + referenceObject.name + " " + referenceObject.TransformToUpdate.name);
        }

        referenceObject.IsReferenceObject = !referenceObject.IsReferenceObject;
    }

    public void SetPersistentObject(DragUI referenceObject)
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

    public void SetOcclusionObject(DragUI element)
    {
        if (element.GetComponent<OcclusionObject>() == null)
        {
            AddOcclusionObject(element);
        }
        else
        {
            RemoveOcclusionObject(element);
        }
    }

    public void AddOcclusionObject(DragUI element)
    {
        OcclusionObject occlusionObject = element.gameObject.AddComponent<OcclusionObject>();
        occlusionObject.SetObjectVisibility(defaultVRScenario.activeInHierarchy ? true : isOcclusionObjVisible);
        occlusionObjects.Add(occlusionObject);
        taskManager.AddPersistentObject(element);
    }

    public void RemoveOcclusionObject(DragUI element)
    {
        OcclusionObject occlusionObject = element.GetComponent<OcclusionObject>();

        occlusionObject.SetObjectVisibility(true);
        occlusionObjects.Remove(occlusionObject);
        taskManager.RmvPersistentObject(element);
        Destroy(occlusionObject);
    }

    public void ToggleOcclusionObjects(bool newValue)
    {
        if (defaultVRScenario.activeInHierarchy && !newValue) return;

        isOcclusionObjVisible = newValue;


        occlusionObjects.ForEach(e => {
            Outline outline = e.GetComponentInChildren<Outline>();

            if (outline != null) outline.IsActive = false;

            e.SetObjectVisibility(isOcclusionObjVisible);
        });

        EventManager.TriggerToggleObjectVisibility(isOcclusionObjVisible);
    }

    private void OnObjectDeleted(GameObject instance)
    {
        OcclusionObject occlusionObject = instance.GetComponentInChildren<OcclusionObject>();

        if (occlusionObject == null) return;

        occlusionObjects.Remove(occlusionObject);
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
    public bool IsResetting { get => toReset; }

    private void OnEnable()
    {
        InputController.Instance.OnStartButtonPressed.AddListener(OnStartButtonClicked);
        InputController.Instance.OnOneButtonPressed.AddListener(OnButtonOneClicked);
        InputController.Instance.OnTwoButtonPressed.AddListener(OnButtonTwoClicked);
        InputController.Instance.OnRightHandTriggerUp.AddListener(OnRightHandTriggerClicked);

        EventManager.OnObjectDeleted += OnObjectDeleted;
    }


    private void OnDisable()
    {
        InputController.Instance.OnStartButtonPressed.RemoveListener(OnStartButtonClicked);
        InputController.Instance.OnOneButtonPressed.RemoveListener(OnButtonOneClicked);
        InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
        InputController.Instance.OnRightHandTriggerUp.RemoveListener(OnRightHandTriggerClicked);

        EventManager.OnObjectDeleted -= OnObjectDeleted;
    }

    public void Exit()
    {
        Application.Quit();
    }
}