using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Plan;
using UnityEngine.UI;
using System;

public class TaskManager : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private GameObject view;

    [Header("Scene References")]
    [SerializeField] private SpatialAnchorManager _spatialAnchorManager;
    [SerializeField] private SceneOriginManager _sceneOriginManager;

    [Header("Buttons")]
    [SerializeField] private UIButton btnNext;
    [SerializeField] private UIButton btnBack;
    [SerializeField] private Toggle toggleTaskStatus;

    [Header("Task Info")]
    [SerializeField] private Transform painelTransform;
    [SerializeField] private Transform libraryTransform;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI taskAmount;
    [SerializeField] private TextMeshProUGUI taskName;

    [Header("Task Config")]
    [SerializeField] private Transform customObjectsParent;
    [SerializeField] private Transform persistentObjectsParent;

    [SerializeField] private RectTransform completedTaskParent;

    [Header("UI")]
    [SerializeField] private GameObject editOptionsContainer;

    [SerializeField] private GameObject configPainel;
    [SerializeField] private UIButton editModeToggle;

    private Plan plan;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Next();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Back();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveTask();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddTask();
        }
    }

    public void Next()
    {
        if (plan.CurrentTaskIndex == plan.TaskAmount - 1) return;

        UpdateTask(plan.CurrentTask, plan.Next());
        EventManager.TriggerStageChange();
    }
    public void Back()
    {
        if (plan.CurrentTaskIndex == 0) return;

        UpdateTask(plan.CurrentTask, plan.Back());
        EventManager.TriggerStageChange();
    }

    public void SaveCurrentStage(Task currentStage)
    {
        Debug.Log($"[TaskManager][SaveCurrentStage] - Salvando etapa atual ... ({customObjectsParent.childCount} objetos)");

        currentStage.sceneElements = new List<SceneElement>();

        // Salva objetos da tarefa atual
        foreach (Transform element in customObjectsParent)
        {
            if (element.childCount == 0 || element.name == "NULL" || element.name.Contains("IGNORE"))
            {
                Debug.Log($"[TaskManager][SaveCurrentStage]         Scene Element vazio");
                continue;
            }

            Debug.Log($"[TaskManager][SaveCurrentStage] - Salvando Scene Element {element.name}");

            try
            {
                SceneElement sceneElement = new SceneElement(element);
                sceneElement.positon = element.localPosition;
                sceneElement.eulerAngles = element.localEulerAngles;

                currentStage.sceneElements.Add(sceneElement);
            
            }catch(Exception error)
            {
                Debug.LogError($"[TaskManager][SaveCurrentStage] Erro ao salvar elemento de cena: {error.Message}");
                Debug.LogError($"[TaskManager][SaveCurrentStage] {error.StackTrace}");
            }
        }

        Debug.Log("[TaskManager][SaveCurrentStage] - Etapa Salva com sucesso!");
    }

    public void UpdateTask(Task oldtTask, Task task)
    {
        Debug.Log("[TaskManager][UpdateTask] Atualizando Etapa atual ...");

        if (oldtTask != null)
        {
            SaveCurrentStage(oldtTask);
        }

        // Remove objetos da tarefa anterior
        foreach (Transform element in customObjectsParent)
        {
            Debug.Log($"[TaskManager][UpdateTask] Remove objeto {element.name}");
            element.name = "NULL";
            Destroy(element.gameObject);
        }

        int counter = 0;
        // Instancia novos objetos da tarefa atual
        foreach (SceneElement sceneElement in task.sceneElements)
        {
            Debug.Log($"[TaskManager][UpdateTask] Instancia sceneElement {counter}");
            Transform sceneElementInstance = new GameObject($"SceneElement {++counter}").transform;
            sceneElementInstance.SetParent(customObjectsParent);
            sceneElementInstance.localPosition = sceneElement.positon;
            sceneElementInstance.localEulerAngles = sceneElement.eulerAngles;

            // Instancia novos objetos da tarefa atual
            foreach (TaskElement element in sceneElement.taskElements)
            {
                Debug.Log($"[TaskManager][UpdateTask]       Instancia element {element.assetName}");
                InstantiateTaskELement(element, sceneElementInstance);
            }
        }


        Debug.Log($"[TaskManager][UpdateTask] Objetos atuais em customObjectsParent {customObjectsParent.childCount}");

        Debug.Log($"[TaskManager][UpdateTask] Atualiza informacoes da tarefa atual");
        // Atualiza informacoes da tarefa atual
        description.text = plan.Name;
        taskAmount.text = $"{plan.CurrentTaskIndex + 1}/{plan.TaskAmount}";

        Debug.Log($"[TaskManager][UpdateTask] Atualiza botoes");
        // Atualiza botoes
        btnBack.Interactable = plan.CurrentTaskIndex != 0;
        btnNext.Interactable = !plan.Finished;

        UpdateProgressionBar(task);

        Debug.Log($"[TaskManager][UpdateTask] Atualiza tarefa atual");
        plan.CurrentTask = task;

        Debug.Log("[TaskManager][UpdateTask] Etapa Atualizada");
    }

    public void CompleteTask(bool newValue)
    {
        plan.SetTaskCompleted(newValue);

        if (plan.Finished || !newValue)
            UpdateProgressionBar(plan.CurrentTask);
        else
            Next();
    }

    public void SaveCurrentStage()
    {
        Debug.Log($"[TaskManager][SaveCurrentStage] Salva tarefa atual ...");
        SaveCurrentStage(plan.CurrentTask);
    }

    private void InstantiateTaskELement(TaskElement element, Transform relativeParent)
    {
        Transform instance = null;

        try
        {
            Debug.Log("[TaskManager][InstantiateTaskELement] Criando obj " + element.assetName + "  " + element.position);

            if (element.objType == TaskElement.ObjType.Model)
            {
                instance = ObjectManager.Instance.InstantiateObject(element.assetName).transform;
                instance.gameObject.AddComponent<InteractableObjectTrackable>();
            }
            else
            {
                instance = ObjectManager.Instance.InstantiateObject("Painel Multimidia").transform;

                instance.GetComponent<WorldCanvasInteractor>().SetNewSize(element.scale);
                instance.GetComponent<PainelController>().LoadFile(element.assetName);
            }

            if (element.persistentTrackableData != null && element.persistentTrackableData.isTracking)
            {
                instance.GetComponent<ITrackebleObject>().LoadData(element.persistentTrackableData);
            }

            instance.name = element.assetName;

            Utils.SetLayer(instance, "InteractableObject", false);

            instance.gameObject.SetActive(true);
            DragUI mainObject = instance.GetComponent<DragUI>();

            mainObject.gameObject.AddComponent<Outline>();
            //mainObject.gameObject.GetComponent<Outline>().enabled = false;
            mainObject.SetNewTransform(relativeParent);
            mainObject.IsFixed = element.isFixed;
            mainObject.IsOcclusion = element.isOcclusionObj;
            mainObject.IsInteractionDisabled = element.isInteractionDisabled;

            if (element.colors != null)
                mainObject.ColorController.UpdateColors(element.colors);


            instance.SetParent(relativeParent);
            instance.localPosition = element.position;
            instance.localRotation = Quaternion.Euler(element.eulerRotation);
            
            if (element.objType == TaskElement.ObjType.Model) instance.localScale = element.scale;

        }
        catch (Exception exp)
        {
            Debug.Log("[TaskManager][InstantiateTaskELement][ERRO] Erro ao instanciar objeto: " + exp.Message);
            Debug.Log("[TaskManager][InstantiateTaskELement][ERRO] Stack Trace: " + exp.StackTrace);

            if (instance != null)
                Destroy(instance.gameObject);
        }
    }

    public void SavePlan(bool triggerSnapshotEvent=true)
    {
        Debug.Log($"[TaskManager][SavePlan] Salvando plano ...");
        if (triggerSnapshotEvent)
        {
            Debug.Log($"[TaskManager][SavePlan]     Salva plano atual no historico");
            EventManager.TriggerTaskChanged(plan.Copy());
        }

        Debug.Log($"[TaskManager][SavePlan]     Salva Tarefa atual");
        SaveCurrentStage(plan.CurrentTask);
        Debug.Log($"[TaskManager][SavePlan]     Salva Objetos persistentes");
        SavePersistentObject();

        Debug.Log($"[TaskManager][SavePlan]     Salva posicao da biblioteca e painel de controle");
        plan.PainelPosition = painelTransform.localPosition;
        plan.PainelRotation = painelTransform.localEulerAngles;

        plan.LibraryPosition = libraryTransform.localPosition;
        plan.LibraryRotation = libraryTransform.localEulerAngles;

        //SessionManager.Instance.SaveCurrentSession();
    }

    private void SavePersistentObject()
    {
        Debug.Log($"[TaskManager][SaveFixedObject] Salvando objetos fixos ... ({persistentObjectsParent.childCount})");

        int i = 0;
        plan.ClearFixedElements();
        foreach (Transform child in persistentObjectsParent)
        {
            if (child.name == "NULL") continue;

            plan.AddFixedElement(new SceneElement(child));
            Debug.Log($"[TaskManager][SaveFixedObject] Salvando objeto fixo {child.name} ({i++}/{persistentObjectsParent.childCount})");
        }

        Debug.Log($"[TaskManager][SaveFixedObject] Objetos fixos salvos com sucesso!");
    }

    public void UpdateProgressionBar(Task currentTask)
    {
        toggleTaskStatus.SetIsOnWithoutNotify(currentTask.finished);
        taskName.text = currentTask.description;
    }

    public void AddObjectToAllStages(GameObject obj)
    {
        foreach (Task task in plan.Tasks)
        {
            if (task == plan.CurrentTask) continue;

            task.sceneElements.Add(new SceneElement(obj.transform));
        }
    }

    public void ChangeTaskName()
    {
        //VRKeyboardManager.Instance.GetUserInputString(ChangeTaskNameCompleted, plan.CurrentTask.description);
        KeyboardManager.Instance.GetInput(ChangeTaskNameCompleted, null, plan.CurrentTask.description);
    }

    public void ChangeTaskNameCompleted(string newTaskName)
    {
        plan.SetTaskName(newTaskName);

        UpdateTask(null, plan.CurrentTask);
    }

    public void LoadFromJson(string jsonContent)
    {
        Plan plan = JsonUtility.FromJson<Plan>(jsonContent);

        _spatialAnchorManager.LoadAnchor(plan.SpatialAnchorOriginID, (pos, rot) => OculusManager.Instance.SetEnvironmentPosition(pos, rot), () => _sceneOriginManager.SetSettingOriginMode(true));

        //plan.Reset();
        LoadPlan(plan);
    }

    public void LoadPlan(Plan newPlan)
    {
        plan = newPlan;
        plan.CurrentTaskIndex = 0;

        LoadTaskManagerPosition();
        LoadFixedObject();
        UpdateTask(null, plan.CurrentTask);
    }

    public void AddObjectInTask(Transform newObj)
    {
        newObj.parent = customObjectsParent;
    }

    public void AddObjectInTask(int index, Transform newObj)
    {
        if (index < 0 || index >= plan.TaskAmount)
        {
            Debug.LogError($"[TaskManager][AddObjectInTask] Index out of range: {index} of {plan.TaskAmount} step amounts");
            return;
        }

        if (index == plan.CurrentTaskIndex)
        {
            AddObjectInTask(newObj);
        }
        else
        {
            try
            {
                plan.Tasks[index].sceneElements.Add(new SceneElement(newObj));
            
            }catch (Exception error)
            {
                Debug.LogError($"[TaskManager][AddObjectInTask] Error during add object in step: {error.Message}");
            }
        }
    }

    public void AddPersistentObject(Transform objParent)
    {
        objParent.SetParent(persistentObjectsParent);
        SavePersistentObject();
    }

    public void RmvPersistentObject(Transform objParent)
    {
        objParent.SetParent(customObjectsParent);
        SavePersistentObject();
    }

    public void SetTaskManagerVisibility(bool newValue)
    {
        view.SetActive(newValue);
    }

    public void AddTask()
    {
        UpdateTask(plan.CurrentTask, plan.AddTask(new Task("")));
    }

    public void RemoveTask()
    {
        UpdateTask(null, plan.RemoveTask(plan.CurrentTaskIndex));
    }

    private void OnEditModeChange(bool isEditMode)
    {
        editOptionsContainer.SetActive(isEditMode);
        editModeToggle.IsOn = isEditMode;
    }

    private void OnObjectDragEnd(ObjectSelector controller, DragUI obj)
    {
        Debug.Log($"[TaskManager][OnObjectDragEnd] Evento de objeto solto recebido");

        if (libraryTransform.hasChanged)
        {
            Debug.Log($"[TaskManager][OnObjectDragEnd]      Salva posicao da Biblioteca");
            plan.LibraryPosition = libraryTransform.localPosition;
            plan.LibraryRotation = libraryTransform.localEulerAngles;
            libraryTransform.hasChanged = false;
        }

        if (painelTransform.hasChanged)
        {
            Debug.Log($"[TaskManager][OnObjectDragEnd]      Salva posicao do Painel de controle");
            plan.PainelPosition = painelTransform.localPosition;
            plan.PainelRotation = painelTransform.localEulerAngles;
            painelTransform.hasChanged = false;
        }


        Debug.Log($"[TaskManager][OnObjectDragEnd]      Salva plano");
        SavePlan();
    }

    public void LoadTaskManagerPosition()
    {
        Debug.Log($"[TaskManager][LoadTaskManagerPosition]  Carrega Posicao do painel e da biblioteca");
        painelTransform.localPosition = plan.PainelPosition;
        painelTransform.localRotation = Quaternion.Euler(plan.PainelRotation);

        libraryTransform.localPosition = plan.LibraryPosition;
        libraryTransform.localRotation = Quaternion.Euler(plan.LibraryRotation);
    }

    public void LoadFixedObject()
    {
        Debug.Log($"[TaskManager][LoadFixedObject] Instanciando objectos fixos ({plan.FixedElements.Count}) ...");

        //Utils.ClearChilds(persistentObjectsParent);
        foreach (Transform child in persistentObjectsParent)
        {
            Debug.Log($"[TaskManager][LoadFixedObject]      Remove {child.name}");
            child.name = "NULL";
            Destroy(child.gameObject);
        }

        foreach (SceneElement sceneElement in plan.FixedElements)
        {
            Debug.Log($"[TaskManager][LoadFixedObject]  Cria sceneElement fixo");
            Transform sceneElementParent = new GameObject("SceneElemenet").transform;
            sceneElementParent.SetParent(persistentObjectsParent);
            sceneElementParent.localPosition = sceneElement.positon;
            sceneElementParent.localEulerAngles = sceneElement.eulerAngles;

            foreach (TaskElement element in sceneElement.taskElements)
            {
                Debug.Log($"[TaskManager][LoadFixedObject]      Cria objeto {element.assetName}");
                InstantiateTaskELement(element, sceneElementParent);
            }
        }

        Log("LoadFixedObject", "Objectos fixos instanciados!");
    }

    public void ToggleConfigPainel(bool newValue)
    {
        configPainel.SetActive(newValue);
    }

    public void ToggleConfigPainel()
    {
        ToggleConfigPainel(!configPainel.activeInHierarchy);
    }

    public Plan GetPlan()
    {
        return plan;
    }

    private void OnObjectDragBegin(ObjectSelector controller, DragUI obj)
    {
        //EventManager.TriggerTaskChanged(plan.CurrentTask.Copy(), plan.FixedElements);
    }

    private void OnEnable()
    {
        //InputController.Instance.OnFourButtonPressed.AddListener(ResetTasks);
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        EventManager.OnExecModeChange += OnEditModeChange;
        SpeechCommandsManager.Instance.OnCommandNext += Next;
        SpeechCommandsManager.Instance.OnCommandBack += Back;
        //EventManager.OnToggleObjectVisibility += OnToggleObjectVisibility;
    }

    private void OnDisable()
    {
        //InputController.Instance.OnFourButtonPressed.RemoveListener(ResetTasks);
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        EventManager.OnExecModeChange -= OnEditModeChange;
        SpeechCommandsManager.Instance.OnCommandNext -= Next;
        SpeechCommandsManager.Instance.OnCommandBack -= Back;
        //EventManager.OnToggleObjectVisibility -= OnToggleObjectVisibility;
    }

    private static void Log(string method, string message) => Debug.Log($"[TaskManager][{method}] {message}");
}