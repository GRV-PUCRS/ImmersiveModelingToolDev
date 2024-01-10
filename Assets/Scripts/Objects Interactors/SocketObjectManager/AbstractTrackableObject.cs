using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ITrackebleObject;

public abstract class AbstractTrackableObject : MonoBehaviour, ITrackebleObject, IExternalDataListener
{
    private const string UI_PREFAB_PATH = "Features/TrackableObjectUI";

    private static int OBJ_COUNTER = 0;

    private bool isTracking = false;
    private SocketObjectUIController uiController;

    private string objectID = "";
    public string ObjectID { get => objectID; private set => objectID = value; }

    public void SetTrackingAttribute(string attributeID, bool value)
    {
        TrackableAttribute attr = GetTrackableAttributes().Find(ta => ta.MethodName.Equals(attributeID));

        if (attr == null) return;

        attr.IsTracking = value;
    }

    public void Active()
    {
        uiController.SetView(true);
    }

    public void SetID(string newID)
    {
        ObjectID = newID;
    }

    public string GetID()
    {
        return ObjectID;
    }

    public void ReceivedData(string data)
    {
        if (!isTracking) return;

        Debug.Log($"[SocketObject] ReceivedData {data}", this);

        try
        {
            SocketMessage message = JsonUtility.FromJson<SocketMessage>(data);

            if (message.id != ObjectID) return;

            var trackableAttributes = GetTrackableAttributes();

            foreach (var attrMessage in message.messages)
            {
                Debug.Log(attrMessage.methodName, this);

                TrackableAttribute trigger = trackableAttributes.Find(t => t.MethodName.Equals(attrMessage.methodName));

                if (trigger == null) continue;

                if (!trigger.IsTracking) continue;

                if (trigger.ArgsType.Count != attrMessage.values.Length) continue;

                object[] args = new object[trigger.ArgsType.Count];

                for (int i = 0; i < trigger.ArgsType.Count; i++)
                {
                    switch (trigger.ArgsType[i])
                    {
                        case "int": args[i] = int.Parse(attrMessage.values[i]); break;
                        case "float": args[i] = float.Parse(attrMessage.values[i]); break;
                        case "bool": args[i] = bool.Parse(attrMessage.values[i]); break;
                        case "string":
                        default: args[i] = attrMessage.values[i]; break;
                    }
                }

                TriggerMethod(attrMessage.methodName, args);
            }

        }
        catch (Exception error)
        {
            Debug.LogError($"[AbstractTrackableObject][{name}] {error.Message}", this);
        }
    }

    public bool IsTracking() => isTracking;

    public void StartTracking()
    {
        // Instantiate UI
        GameObject uiPrefab = Resources.Load<GameObject>(UI_PREFAB_PATH);
        uiController = Instantiate(uiPrefab, transform).GetComponent<SocketObjectUIController>();
        uiController.gameObject.name = "IGNORE";
        //uiController.transform.eulerAngles += Vector3.up * 180;

        uiController.SetAttributes(this);
        uiController.SetView(true);

        uiController.SetObjectID(objectID.Length == 0 ? $"obj_{OBJ_COUNTER++}" : objectID);

        ExternalDataManager.Instance.AddListener(this);
        GetObjectController().OnDestroyedObject += StopTracking;

        isTracking = true;
        Debug.Log($"[AbstractTrackableObject] {ObjectID} start tracking");
    }

    public void StopTracking()
    {
        isTracking = false;

        ExternalDataManager.Instance.RmvListener(this);

        if (uiController != null) Destroy(uiController.gameObject);
    }

    public Transform GetUITransform() => uiController.transform;

    public void SetActiveUI(bool value)
    {
        uiController.SetView(value);
    }

    public void LoadData(PersistentTrackableData data)
    {
        Debug.Log($"[AbstractTrackableObject] Load Data ...");

        if (data == null) return;

        objectID = data.objectId;

        if (data.methods != null)
        {
            foreach (var attr in data.methods)
            {
                SetTrackingAttribute(attr, true);
            }
        }

        StartTracking();
        GetUITransform().SetParent(transform.parent.parent);
        SetActiveUI(false);


        Debug.Log("[AbstractTrackableObject] Data loaded");
    }

    public PersistentTrackableData SaveData()
    {
        return new PersistentTrackableData(this);
    }

    private void OnDestroy()
    {
        try
        {
            ExternalDataManager.Instance.RmvListener(this);
        }
        catch (Exception error)
        {
            Debug.LogError($"[AbstractTrackableObject] Error during cleaning up Socket Object: {error.Message}", this);
        }
    }


    #region abstract_methods
    public abstract DragUI GetObjectController();

    public abstract List<TrackableAttribute> GetTrackableAttributes();

    public abstract void TriggerMethod(string methodName, object[] args);
    #endregion

    #region private_classes
    [Serializable]
    private class SocketMessage
    {
        public string id;
        public AttributeMessage[] messages;
    }

    [Serializable]
    private class AttributeMessage
    {
        public string methodName;
        public string[] argsType;
        public string[] values;
    }
    #endregion
}
