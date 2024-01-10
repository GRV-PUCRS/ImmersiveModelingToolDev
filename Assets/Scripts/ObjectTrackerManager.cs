using System.Collections.Generic;
using UnityEngine;

public class ObjectTrackerManager : Singleton<ObjectTrackerManager>
{
    public enum ObjectTrackerMessageType { UPDATE_OBJECT=0 }

    private Dictionary<string, ObjectTracker> objectsInstances;
    private int idCounter;

    private void Awake()
    {
        objectsInstances = new Dictionary<string, ObjectTracker>();
        idCounter = 0;
    }

    private void Start()
    {
        EventManager.OnStageChange += EventManager_OnStageChange;
        NetworkPhoton.Instance.OnObjectTrackerMessage.AddListener(HandleObjectTrackerMessage);
    }

    public string AddObject(ObjectTracker objTracker)
    {
        if (!NetworkPhoton.Instance.InRoom)
        {
            Debug.LogWarning($"[{this}][AddObject] Application not connected in a Room", this);
            return "";
        }

        string objectID = $"OBJ_TRACKER_{idCounter}";

        objectsInstances.Add(objectID, objTracker);

        return objectID;
    }

    public void RemoveObject(ObjectTracker objTracker)
    {
        if (!objectsInstances.ContainsKey(objTracker.ObjectID)) return;

        objectsInstances.Remove(objTracker.ObjectID);
    }

    private void HandleUpdateObject(object[] message)
    {
        if (message.Length != 5)
        {
            Debug.LogWarning($"[{this}][HandleUpdateObject] Object Tracker message with bad format: {message.Length} values", this);
            return;
        }

        string objectID = (string)message[1];
        Vector3 position = (Vector3)message[2];
        Vector3 eulerAngle = (Vector3)message[3];
        Vector3 scale = (Vector3)message[4];

        if (!objectsInstances.ContainsKey(objectID))
        {
            Debug.LogWarning($"[{this}][HandleUpdateObject] ObjectTracker with ID {objectID} not found", this);
            return;
        }

        objectsInstances[objectID].UpdateTransform(position, eulerAngle, scale);
    }

    private void HandleObjectTrackerMessage(object[] message)
    {
        ObjectTrackerMessageType msgType = (ObjectTrackerMessageType) message[0];

        switch (msgType)
        {
            case ObjectTrackerMessageType.UPDATE_OBJECT:
                HandleUpdateObject(message);

                break;

            default:
                break;
        }
    }

    private void EventManager_OnStageChange()
    {
        idCounter = 0;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        EventManager.OnStageChange -= EventManager_OnStageChange;
        NetworkPhoton.Instance.OnObjectTrackerMessage.RemoveListener(HandleObjectTrackerMessage);
    }
}
