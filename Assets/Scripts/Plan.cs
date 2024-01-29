using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ITrackebleObject;

[Serializable]
public class Plan
{
    [SerializeField] string name;
    [SerializeField] string spatialAnchorOriginID;
    [SerializeField] List<Task> tasks;
    [SerializeField] private int currentTask;
    [SerializeField] private Vector3 painelPosition;
    [SerializeField] private Vector3 painelRotation;
    [SerializeField] private Vector3 libraryPosition;
    [SerializeField] private Vector3 libraryRotation;
    [SerializeField] private List<SceneElement> fixedElements;

    public Plan(string name)
    {
        this.name = name;

        tasks = new List<Task>();
        tasks.Add(new Task("New Task"));

        spatialAnchorOriginID = "";
        currentTask = 0;
        painelPosition = Vector3.zero;
        painelRotation = Vector3.zero;
        libraryPosition = Vector3.up;
        libraryRotation = Vector3.up * 180;
        fixedElements = new List<SceneElement>();
    }

    public Plan(Plan planToCopy)
    {
        name = planToCopy.name;

        tasks = new List<Task>();
        foreach (Task task in planToCopy.tasks)
            tasks.Add(new Task(task));

        spatialAnchorOriginID = planToCopy.spatialAnchorOriginID;
        currentTask = planToCopy.currentTask;
        painelPosition = planToCopy.painelPosition;
        painelRotation = planToCopy.painelRotation;
        libraryPosition = planToCopy.libraryPosition;
        libraryRotation = planToCopy.libraryRotation;

        fixedElements = new List<SceneElement>();
        foreach (SceneElement element in planToCopy.fixedElements)
            fixedElements.Add(element);
    }

    [Serializable]
    public class Task
    {
        [SerializeField] public List<SceneElement> sceneElements;
        [SerializeField] public string description;
        [SerializeField] public bool finished;

        public Task(string description)
        {
            sceneElements = new List<SceneElement>();
            finished = false;

            this.description = description;
        }

        public Task(Task taskToCopy)
        {
            sceneElements = new List<SceneElement>();
            foreach (SceneElement sceneElement in taskToCopy.sceneElements)
            {
                sceneElements.Add(new SceneElement(sceneElement));
            }

            finished = taskToCopy.finished;
            description = taskToCopy.description;
        }

        
    }
    public EditorSnapshot GetSnapshot()
    {
        EditorSnapshot editorSnapshot = new EditorSnapshot();

        editorSnapshot.Name = name;
        editorSnapshot.Task = new Task(CurrentTask);
        editorSnapshot.FixedElements = new List<SceneElement>();

        foreach (SceneElement fixedElement in fixedElements)
        {
            editorSnapshot.FixedElements.Add(new SceneElement(fixedElement));
        }

        editorSnapshot.LibraryPosition = libraryPosition;
        editorSnapshot.LibraryRotation = libraryRotation;
        editorSnapshot.TaskManagerPosition = painelPosition;
        editorSnapshot.TaskManagerRotation = painelRotation;

        return editorSnapshot;
    }

    public class EditorSnapshot
    {
        public string Name { get; set; }
        public Task Task { get; set; }
        public List<SceneElement> FixedElements { get; set; }
        public Vector3 LibraryPosition { get; set; }
        public Vector3 LibraryRotation { get; set; }
        public Vector3 TaskManagerPosition { get; set; }
        public Vector3 TaskManagerRotation { get; set; }
    }

    [Serializable]
    public class SceneElement
    {
        [SerializeField] public List<TaskElement> taskElements;
        [SerializeField] public Vector3 positon;
        [SerializeField] public Vector3 eulerAngles;

        public SceneElement(Transform sceneElement)
        {
            taskElements = new List<TaskElement>();

            Debug.Log($"SceneElement {sceneElement.name}");
            foreach (Transform child in sceneElement)
            {
                Debug.Log($"    Task Element {child.name}");

                taskElements.Add(new TaskElement(child.gameObject));
            }

            positon = sceneElement.localPosition;
            eulerAngles = sceneElement.localEulerAngles;
        }

        public SceneElement(SceneElement sceneElementToCopy)
        {
            taskElements = new List<TaskElement>();

            foreach (TaskElement taskElement in sceneElementToCopy.taskElements)
            {
                taskElements.Add(new TaskElement(taskElement));
            }

            positon = sceneElementToCopy.positon;
            eulerAngles = sceneElementToCopy.eulerAngles;
        }
    }

    [Serializable]
    public class TaskElement
    {
        public enum ObjType { Painel, Model, CustomTextPainel };

        public Vector3 position;
        public Vector3 eulerRotation;
        public Vector3 scale;
        public string assetName;
        public ObjType objType;
        public bool isFixed;
        public bool isOcclusionObj;
        public bool isInteractionDisabled;
        public List<Vector3> colors;
        public PersistentTrackableData persistentTrackableData;

        public TaskElement(GameObject instance)
        {
            position = instance.transform.localPosition;
            eulerRotation = instance.transform.localEulerAngles;
            scale = instance.transform.localScale;
            assetName = instance.name;

            if (assetName.Contains("."))
            {
                objType = assetName.Contains(".ptx") ? ObjType.CustomTextPainel : ObjType.Painel;

                var rectTransform = instance.GetComponentInChildren<RectTransform>();

                if (rectTransform != null)
                {
                    scale = rectTransform.sizeDelta;
                }
            }
            else
            {
                objType = ObjType.Model;
            }

            DragUI dragUI = instance.GetComponentInChildren<DragUI>();

            isFixed = dragUI.IsFixed;
            isOcclusionObj = dragUI.IsOcclusion;
            isInteractionDisabled = dragUI.IsInteractionDisabled;

            colors = new List<Vector3>();
            foreach (MeshRenderer renderer in dragUI.ColorController.MeshRenderers)
            {
                colors.Add(new Vector3(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b));
            }

            ITrackebleObject trackableObject = instance.GetComponentInChildren<ITrackebleObject>();

            if (trackableObject != null)
            {
                persistentTrackableData = trackableObject.SaveData();

                Debug.Log($"[TEMP] Salva trackable {instance.name} {trackableObject.GetID()}");

                foreach (string a in persistentTrackableData.methods) Debug.Log($"[TEMP]   Salva method {a}");
            }
            else
            {
                Debug.Log($"[TEMP] Nao Salva trackable {instance.name}");
            }
        }

        public TaskElement(TaskElement taskElementToCopy)
        {
            position = taskElementToCopy.position;
            eulerRotation = taskElementToCopy.eulerRotation;
            scale = taskElementToCopy.scale;
            assetName = taskElementToCopy.assetName;
            objType = taskElementToCopy.objType;
            isFixed = taskElementToCopy.isFixed;
            isOcclusionObj = taskElementToCopy.isOcclusionObj;
            isInteractionDisabled = taskElementToCopy.isInteractionDisabled;
            colors = taskElementToCopy.colors;
            persistentTrackableData = taskElementToCopy.persistentTrackableData;
        }
    }

    public Task Next()
    {
        if (currentTask < tasks.Count - 1)
            currentTask++;

        return tasks[currentTask];
    }

    public Task Back()
    {
        if (currentTask > 0)
            currentTask--;

        return tasks[currentTask];
    }

    public Task AddTask(Task newTask)
    {
        if (tasks.Count == 0 || CurrentTaskIndex == tasks.Count - 1)
        {
            tasks.Add(newTask);
        }
        else
        {
            tasks.Insert(CurrentTaskIndex + 1, newTask);
        }

        return CurrentTask;
    }

    public void SetTaskName(string newName)
    {
        CurrentTask.description = newName;
    }

    public Task RemoveTask(int index)
    {
        if (tasks.Count == 1)
            tasks[0] = new Task("");
        else
            tasks.RemoveAt(index);

        return Back();
    }
    public void AddFixedElement(SceneElement newElement)
    {
        fixedElements.Add(newElement);
    }

    public void ClearFixedElements()
    {
        fixedElements.Clear();
    }

    public Plan Copy()
    {
        return new Plan(this);
    }

    public string SpatialAnchorOriginID { get => spatialAnchorOriginID; set => spatialAnchorOriginID = value; }
    public bool Finished { get => currentTask == tasks.Count - 1; }
    public int CurrentTaskIndex { get => currentTask; set => currentTask = value; }
    public int TaskAmount { get => tasks.Count; }
    public string Name { get => name; set => name = value; }
    public void SetTaskCompleted(bool value)
    {
        tasks[currentTask].finished = value;
    }
    public List<Task> Tasks { get => tasks; }
    public Task CurrentTask { get => tasks[currentTask]; set => tasks[currentTask] = value; }
    public Vector3 PainelPosition { get => painelPosition; set => painelPosition = value; }
    public Vector3 PainelRotation { get => painelRotation; set => painelRotation = value; }
    public Vector3 LibraryPosition { get => libraryPosition; set => libraryPosition = value; }
    public Vector3 LibraryRotation { get => libraryRotation; set => libraryRotation = value; }
    public List<SceneElement> FixedElements { get => fixedElements; set => fixedElements = value; }

    public void Reset()
    {
        currentTask = 0;

        foreach (Task task in tasks)
            task.finished = false;
    }
}
