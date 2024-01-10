using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Plan;

public class CommandManager : Singleton<CommandManager>
{
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject btnUndo;

    private Stack<Plan> editorSnapshotHistory;

    private void Awake()
    {
        editorSnapshotHistory = new Stack<Plan>();

        if (taskManager == null) enabled = false;

        btnUndo.SetActive(false);
    }

    public void AddEntry(Plan snapshot)
    {
        btnUndo.SetActive(true);
        editorSnapshotHistory.Push(snapshot);

        Debug.Log($"[CommandManager][AddCommand] Add new Command ({editorSnapshotHistory.Count})");
    }

    public void Undo()
    {
        Debug.Log("[CommandManager][Undo] Remove last command");
        if (editorSnapshotHistory.Count == 0)
        {
            Debug.Log("[CommandManager][Undo]   Stack empty");
            return;
        }

        Plan snapshot = editorSnapshotHistory.Pop();

        taskManager.LoadPlan(snapshot);
        taskManager.SavePlan(false);

        btnUndo.SetActive(editorSnapshotHistory.Count != 0);
        Debug.Log("[CommandManager][Undo] Last command Removed");
    }

    private void OnTaskChanged(Plan snapshot)
    {
        AddEntry(snapshot);
    }

    private void OnStageChange()
    {
        editorSnapshotHistory.Clear();
    }

    private void OnEnable()
    {
        EventManager.OnTaskChanged += OnTaskChanged;
        EventManager.OnStageChange += OnStageChange;
    }


    private void OnDisable()
    {
        EventManager.OnTaskChanged -= OnTaskChanged;
        EventManager.OnStageChange -= OnStageChange;
    }

    

    /*
    private Stack<List<GameObject>> objectsHistory;
    private List<GameObject> currentObjectsChanged;

    private void Awake()
    {
        objectsHistory = new Stack<List<GameObject>>();
        currentObjectsChanged = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Undo();
        }
    }

    public void AddCommand(List<GameObject> objects)
    {
        List<GameObject> references = new List<GameObject>();

        foreach (GameObject sceneElement in objects)
        {
            GameObject sceneElementCopy = Instantiate(sceneElement);

            sceneElementCopy.name = sceneElement.name;
            sceneElementCopy.transform.SetParent(sceneElement.transform.parent);
            sceneElementCopy.transform.localPosition = sceneElement.transform.localPosition;
            sceneElementCopy.transform.localRotation = sceneElement.transform.localRotation;

            DragUI[] oldElements = sceneElement.GetComponentsInChildren<DragUI>();
            DragUI[] newElements = sceneElementCopy.GetComponentsInChildren<DragUI>();

            for (int i = 0; i < oldElements.Length; i++)
            {
                newElements[i].SetNewTransform(oldElements[i].TransformToUpdate);
                newElements[i].IsOcclusion = oldElements[i].IsOcclusion;
                newElements[i].IsFixed = oldElements[i].IsFixed;
                newElements[i].IsPersistent = oldElements[i].IsPersistent;
            }

            sceneElementCopy.SetActive(false);

            references.Add(sceneElementCopy);
        }

        objectsHistory.Push(references);

        GameObject[] temp = new GameObject[objects.Count];
        objects.CopyTo(temp);
        currentObjectsChanged = new List<GameObject>(temp);

        Debug.Log("[CommandManager][AddCommand] Add new Command");
    }

    public void Undo()
    {
        Debug.Log("[CommandManager][Undo] Remove last command");
        if (objectsHistory.Count == 0)
        {
            Debug.Log("[CommandManager][Undo]   Stack empty");
            return;
        }

        List<GameObject> objects = objectsHistory.Pop();

        foreach (GameObject obj in objects)
            obj.SetActive(true);

        foreach (GameObject obj in currentObjectsChanged)
            Destroy(obj);

        Debug.Log("[CommandManager][Undo] Last command Removed");
    }

    private void OnRaiseAction(List<GameObject> objects)
    {
        AddCommand(objects);
    }

    private void OnLeftHandTriggerDown()
    {
        Undo();
    }

    private void OnEnable()
    {
        EventManager.OnRaiseAction += OnRaiseAction;
        InputController.Instance.OnLeftHandTriggerDown.AddListener(OnLeftHandTriggerDown);
    }

    private void OnDisable()
    {
        EventManager.OnRaiseAction -= OnRaiseAction;
        InputController.Instance.OnLeftHandTriggerDown.RemoveListener(OnLeftHandTriggerDown);
    }
    */
    /*
    private Stack<ICommand> commandHistory;
    
    private void Awake()
    {
        commandHistory = new Stack<ICommand>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Undo();
        }
    }

    public void AddCommand(ICommand command)
    {
        commandHistory.Push(command);

        Debug.Log("[CommandManager][AddCommand] Add new Command");
    }

    public void Undo()
    {
        Debug.Log("[CommandManager][Undo] Remove last command");
        if (commandHistory.Count == 0)
        {
            Debug.Log("[CommandManager][Undo]   Stack empty");
            return;
        }

        ICommand command = commandHistory.Pop();
        command.Undo();
        Debug.Log("[CommandManager][Undo] Last command Removed");
    }

    private void OnRaiseCommand(ICommand newCommand)
    {
        AddCommand(newCommand);
    }

    private void OnLeftHandTriggerDown()
    {
        Undo();
    }

    private void OnEnable()
    {
        EventManager.OnRaiseCommand += OnRaiseCommand;
        InputController.Instance.OnLeftHandTriggerDown.AddListener(OnLeftHandTriggerDown);
    }

    private void OnDisable()
    {
        EventManager.OnRaiseCommand -= OnRaiseCommand;
        InputController.Instance.OnLeftHandTriggerDown.RemoveListener(OnLeftHandTriggerDown);
    }
    */
}
