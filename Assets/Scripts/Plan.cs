using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Plan
{
    [SerializeField] string name;
    [SerializeField] List<Task> tasks;
    [SerializeField] private int currentTask;
    [SerializeField] private Vector3 painelPosition;
    [SerializeField] private Vector3 painelRotation;
    [SerializeField] private Vector3 libraryPosition;
    [SerializeField] private Vector3 libraryRotation;
    [SerializeField] private List<TaskElement> fixedElements;

    public Plan()
    {
        tasks = new List<Task>();
        currentTask = -1;
        painelPosition = Vector3.zero;
        painelRotation = Vector3.zero;
        libraryPosition = Vector3.zero;
        libraryRotation = Vector3.zero;
        fixedElements = new List<TaskElement>();
    }

    [Serializable]
    public class Task
    {
        [SerializeField] public List<TaskElement> taskElements;
        [SerializeField] public string description;
        [SerializeField] public bool finished;

        public Task(string description)
        {
            taskElements = new List<TaskElement>();
            finished = false;

            this.description = description;
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

        public TaskElement(Vector3 position, Vector3 eulerRotation, Vector3 scale, string assetName, ObjType objType, bool isFixed, bool isOcclusionObj)
        {
            this.position = position;
            this.eulerRotation = eulerRotation;
            this.scale = scale;
            this.assetName = assetName;
            this.objType = objType;
            this.isFixed = isFixed;
            this.isOcclusionObj = isOcclusionObj;
        }

        public TaskElement(GameObject instance)
        {
            position = instance.transform.localPosition;
            eulerRotation = instance.transform.localEulerAngles;
            scale = instance.transform.localScale;
            assetName = instance.name;

            if (assetName.Contains(".ptx"))
            {
                objType = ObjType.CustomTextPainel;
            }
            else
            {
                objType = assetName.Contains(".") ? ObjType.Painel : ObjType.Model;
            }

            DragUI dragUI = instance.GetComponentInChildren<DragUI>();
            isFixed = dragUI != null ? dragUI.IsReferenceObject : false;
            isOcclusionObj = instance.GetComponentInChildren<OcclusionObject>() != null;
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
            tasks[0] = new Task("Nova Etapa");
        else
            tasks.RemoveAt(index);

        return Back();
    }
    public void AddFixedElement(TaskElement newElement)
    {
        fixedElements.Add(newElement);
    }

    public void ClearFixedElements()
    {
        fixedElements.Clear();
    }

    public bool Finished { get => currentTask == tasks.Count - 1; }
    public int CurrentTaskIndex { get => currentTask; }
    public int TaskAmount { get => tasks.Count; }
    public string Name { get => name; set => name = value; }
    public void SetTaskCompleted(bool value)
    {
        tasks[currentTask].finished = value;
    }
    public List<Task> Tasks { get => tasks; }
    public Task CurrentTask { get => tasks[currentTask]; }
    public Vector3 PainelPosition { get => painelPosition; set => painelPosition = value; }
    public Vector3 PainelRotation { get => painelRotation; set => painelRotation = value; }
    public Vector3 LibraryPosition { get => libraryPosition; set => libraryPosition = value; }
    public Vector3 LibraryRotation { get => libraryRotation; set => libraryRotation = value; }
    public List<TaskElement> FixedElements { get => fixedElements; set => fixedElements = value; }


    public void Reset()
    {
        currentTask = 0;

        foreach (Task task in tasks)
            task.finished = false;
    }
}
