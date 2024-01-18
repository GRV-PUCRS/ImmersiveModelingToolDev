using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class OculusManager : Singleton<OculusManager>
{
    [Header("Referencias")]
    [SerializeField] private Transform environmentParent;
    [SerializeField] private GameObject sceneParent;
    [SerializeField] private OVRPassthroughLayer oVRPassthroughLayer;
    [SerializeField] private GameObject defaultVRScenario;
    [SerializeField] private Transform initialScreen;
    [SerializeField] private GameObject library;
    [SerializeField] private TaskManager taskManager;

    private bool scaleX = true, scaleY = true, scaleZ = true;
    private bool isEditMode = false;
    private bool isOcclusionObjVisible = false;
    private bool toReset = false;

    private List<GameObject> currentSelection;

    private void Awake()
    {
        initialScreen.gameObject.SetActive(false);
        environmentParent.gameObject.SetActive(true);

        currentSelection = new List<GameObject>();
    }

    private void Start()
    {
        ToggleARVRMode(true);
        ToggleEditMode(true);
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

            //InputController.Instance.OnOneButtonPressed.RemoveListener(OnButtonOneClicked);
            InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
        }
        else
        {
            ToggleOcclusionObjects(isOcclusionObjVisible);

            //InputController.Instance.OnOneButtonPressed.AddListener(OnButtonOneClicked);
            InputController.Instance.OnTwoButtonPressed.AddListener(OnButtonTwoClicked);
        }

        EventManager.TriggerVRModeActive(newValue);
    }

    public void ReplicateObject(GameObject sceneElement)
    {
        taskManager.AddObjectToAllStages(sceneElement);
    }
    public void SetPersistentObject(GameObject sceneElement)
    {
        taskManager.AddPersistentObject(sceneElement.transform);
    }

    public void RemovePersistentObject(GameObject sceneElement)
    {
        taskManager.RmvPersistentObject(sceneElement.transform);
    }

    public void AddObjectToTask(Transform obj)
    {
        taskManager.AddObjectInTask(obj);
    }

    public void ResetSession()
    {
        //TODO
    }

    public void ToggleEditMode(bool newValue)
    {
        //library.SetActive(newValue);
        isEditMode = newValue;

        EventManager.TriggerExecModeChange(newValue);

        if (!isEditMode)
            ToggleOcclusionObjects(false);
    }

    public void ToggleOcclusionObjects(bool newValue)
    {
        if (defaultVRScenario.activeInHierarchy && !newValue) return;

        isOcclusionObjVisible = newValue;

        foreach(DragUI element in sceneParent.GetComponentsInChildren<DragUI>())
        {
            Outline outline = element.GetComponentInChildren<Outline>();

            if (outline != null) outline.DisableOutline();

            if (element.IsOcclusion)
            {
                element.SetObjectVisibility(isOcclusionObjVisible);
            }
        }

        //EventManager.TriggerToggleObjectVisibility(isOcclusionObjVisible);
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

    public void AddSelectedObject(GameObject sceneElement) => AddSelectedObject(sceneElement, Color.red);

    public void AddSelectedObject(GameObject sceneElement, Color colorWhenSelected)
    {
        if (currentSelection.Contains(sceneElement)) return;

        ChangeObjectSelectionState(sceneElement, true, colorWhenSelected);

        currentSelection.Add(sceneElement);
    }

    public void RmvSelectedObject(GameObject sceneElement) => RmvSelectedObject(sceneElement, Color.red);

    public void RmvSelectedObject(GameObject sceneElement, Color colorWhenSelected)
    {
        if (!currentSelection.Contains(sceneElement)) return;

        ChangeObjectSelectionState(sceneElement, false, colorWhenSelected);

        currentSelection.Remove(sceneElement);
    }

    private void ChangeObjectSelectionState(GameObject sceneElement, bool isOn) => ChangeObjectSelectionState(sceneElement, isOn, Color.red);

    private void ChangeObjectSelectionState(GameObject sceneElement, bool isOn, Color colorWhenSelected)
    {
        if (sceneElement == null) return;

        foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
        {
            element.IsSelected = isOn;

            Outline outline = element.GetComponent<Outline>();

            if (outline != null)
            {
                outline.DisableOutline();

                if (isOn)
                {
                    outline.OutlineColor = colorWhenSelected;
                    outline.EnableOutline();
                }
                else
                {
                    outline.OutlineColor = Color.white;
                }
            }
        }
    }

    public void ClearSelection()
    {
        foreach (GameObject sceneElement in currentSelection)
            ChangeObjectSelectionState(sceneElement, false);

        currentSelection.Clear();
    }

    public void SetHandTrackingActive(bool value)
    {
        HandTrackingManager.Instance.SetHandActive(value);
    }

    public void SetEnvironmentPosition(Vector3 position, Quaternion rotation)
    {
        environmentParent.position = position;
        environmentParent.rotation = rotation;

        EventManager.TriggerOriginChange(environmentParent.transform);
    }

    public Transform GetEnvironmentTransform() => environmentParent;

    public bool ScaleX { get => scaleX; }
    public bool ScaleY { get => scaleY; }
    public bool ScaleZ { get => scaleZ; }
    public bool IsEditMode { get => isEditMode; }
    public bool IsOcclusionObjVisible { get => isOcclusionObjVisible; }
    public bool IsResetting { get => toReset; }
    public bool IsInVRMode { get => defaultVRScenario.activeInHierarchy; }
    public List<GameObject> SelectionList { get => currentSelection; }

    public TaskManager TaskManager { get => taskManager; }

    public Transform Origin { get => environmentParent.transform; }

    private void OnEnable()
    {
        InputController.Instance.OnTwoButtonPressed.AddListener(OnButtonTwoClicked);
    }


    private void OnDisable()
    {
        InputController.Instance.OnTwoButtonPressed.RemoveListener(OnButtonTwoClicked);
    }

    public void Exit()
    {
        Application.Quit();
    }
}