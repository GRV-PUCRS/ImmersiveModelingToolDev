using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Plan;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using System;

public class TaskManager : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private GameObject view;

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
    private int maxTaskVisivle = 5;

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
        Debug.Log($"[TaskManager][SaveCurrentStage] - Salvando etapa atual ... ({currentStage.taskElements.Count} objetos)");

        currentStage.taskElements = new List<TaskElement>();

        // Salva objetos da tarefa atual
        foreach (Transform element in customObjectsParent)
        {
            Debug.Log("[TaskManager][SaveCurrentStage] - Salvando obj " + element.name);
            currentStage.taskElements.Add(new TaskElement(element.gameObject));
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
            Destroy(element.gameObject);
        }

        // Instancia novos objetos da tarefa atual
        foreach (TaskElement element in task.taskElements)
        {
            InstantiateTaskELement(element, customObjectsParent);
        }

        // Atualiza informacoes da tarefa atual
        description.text = plan.Name;
        taskAmount.text = $"{plan.CurrentTaskIndex + 1}/{plan.TaskAmount}";

        // Atualiza botoes
        btnBack.Interactable = plan.CurrentTaskIndex != 0;
        btnNext.Interactable = !plan.Finished;

        UpdateProgressionBar(task);
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
        SaveCurrentStage(plan.CurrentTask);
    }

    private void InstantiateTaskELement(TaskElement element, Transform relativeParent)
    {
        Transform instance = null;
        Transform parent = null;

        try
        {
            parent = new GameObject().transform;
            parent.name = element.assetName;
            parent.SetParent(relativeParent);

            parent.localPosition = element.position;
            parent.localRotation = Quaternion.Euler(element.eulerRotation);

            Debug.Log("[TaskManager][UpdateTask] Criando obj " + element.assetName + "  " + element.position);

            if (element.objType == TaskElement.ObjType.Model)
            {
                if (!ObjectManager.Instance.ContainsObject(element.assetName))
                {
                    throw new Exception("Objeto " + element.assetName + " nao encontrado!");
                }

                instance = ObjectManager.Instance.InstantiateObject(element.assetName).transform;
                Utils.SetLayer(instance, "InteractableObject", false);
            }
            else
            {
                if (!ObjectManager.Instance.ContainsObject("Text Painel"))
                {
                    throw new Exception("Objeto 'painel' nao encontrado!");
                }

                instance = ObjectManager.Instance.InstantiateObject("Text Painel").transform;

                if (element.objType == TaskElement.ObjType.CustomTextPainel)
                {
                    instance.GetComponent<PainelController>().SetTextPainel(element.assetName.Replace(".ptx", ""));
                }
                else
                {
                    instance.GetComponent<PainelController>().LoadFile(new ObjectFile(element.assetName));
                }
            }

            DragUI mainObject = instance.GetComponent<DragUI>();
            mainObject.gameObject.AddComponent<Outline>();
            mainObject.gameObject.GetComponent<Outline>().enabled = false;
            mainObject.SetNewTransform(parent);


            if (element.isFixed)
                OculusManager.Instance.SetObjectAsOrigin(mainObject);

            if (element.isOcclusionObj)
                OculusManager.Instance.SetOcclusionObject(mainObject);

            instance.SetParent(parent);
            instance.localPosition = Vector3.zero;
            instance.localRotation = Quaternion.identity;
            parent.localScale = element.scale;

        }
        catch (Exception exp)
        {
            Debug.Log("[TaskManager][UpdateTask][ERRO] Erro ao instanciar objeto: " + exp.Message);

            if (instance != null)
                Destroy(parent.gameObject);
        }
    }

    public void SavePlan()
    {
        SaveCurrentStage(plan.CurrentTask);
        SavePersistentObject();

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
            plan.AddFixedElement(new TaskElement(child.gameObject));
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
            task.taskElements.Add(new TaskElement(obj));
        }
    }

    public void ChangeTaskName()
    {
        VRKeyboardManager.Instance.GetUserInputString(ChangeTaskNameCompleted, plan.CurrentTask.description);
    }

    public void ChangeTaskNameCompleted(string newTaskName)
    {
        plan.SetTaskName(newTaskName);

        UpdateTask(null, plan.CurrentTask);
    }

    public void LoadFromJson(string jsonContent)
    {
        plan = JsonUtility.FromJson<Plan>(jsonContent);

        if (plan.Tasks.Count == 0)
        {
            plan.AddTask(new Task("Nova Etapa"));
        }

        plan.Reset();

        LoadTaskManagerPosition();
        LoadFixedObject();
        UpdateTask(null, plan.CurrentTask);
    }

    public void AddObjectInTask(Transform newObj)
    {
        newObj.parent = customObjectsParent;
    }

    public void AddPersistentObject(DragUI newObj)
    {
        newObj.TransformToUpdate.SetParent(persistentObjectsParent);
    }

    public void RmvPersistentObject(DragUI oldObj)
    {
        oldObj.TransformToUpdate.SetParent(customObjectsParent);
    }

    public void SetTaskManagerVisibility(bool newValue)
    {
        view.SetActive(newValue);
    }

    public void AddTask()
    {
        UpdateTask(plan.CurrentTask, plan.AddTask(new Task("New Step")));
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

    private void OnObjectDragEnd(Transform obj)
    {
        SavePlan();
    }

    public void LoadTaskManagerPosition()
    {
        painelTransform.localPosition = plan.PainelPosition;
        painelTransform.localRotation = Quaternion.Euler(plan.PainelRotation);

        libraryTransform.localPosition = plan.LibraryPosition;
        libraryTransform.localRotation = Quaternion.Euler(plan.LibraryRotation);
    }

    public void LoadFixedObject()
    {
        Debug.Log($"[TaskManager][LoadFixedObject] Instanciando objectos fixos ({plan.FixedElements.Count}) ...");

        Utils.ClearChilds(persistentObjectsParent);

        foreach (TaskElement element in plan.FixedElements)
        {
            InstantiateTaskELement(element, persistentObjectsParent);
        }

        Debug.Log($"[TaskManager][LoadFixedObject] Objetos fixos instanciandos!");
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

    private void OnEnable()
    {
        //InputController.Instance.OnFourButtonPressed.AddListener(ResetTasks);
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        OculusManager.OnEditModeChange += OnEditModeChange;
        //EventManager.OnToggleObjectVisibility += OnToggleObjectVisibility;
    }

    private void OnDisable()
    {
        //InputController.Instance.OnFourButtonPressed.RemoveListener(ResetTasks);
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        OculusManager.OnEditModeChange -= OnEditModeChange;
        //EventManager.OnToggleObjectVisibility -= OnToggleObjectVisibility;
    }
}