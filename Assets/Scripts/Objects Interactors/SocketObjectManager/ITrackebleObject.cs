using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrackebleObject
{
    bool IsTracking();
    void StartTracking();
    void StopTracking();
    void SetActiveUI(bool value);
    Transform GetUITransform();

    List<TrackableAttribute> GetTrackableAttributes();
    void TriggerMethod(string methodName, object[] args);
    void SetTrackingAttribute(string methoname, bool isListener);
    DragUI GetObjectController();
    void SetID(string newID);
    string GetID();
    void LoadData(PersistentTrackableData data);
    PersistentTrackableData SaveData();

    public class TrackableAttribute
    {
        public string MethodName { get; }
        public List<string> ArgsType { get; }
        public bool IsTracking { get; set; }
    
        public TrackableAttribute(string methodName, List<string> argsType, bool isTracking)
        {
            MethodName = methodName;
            ArgsType = argsType;
            IsTracking = isTracking;
        }
    }

    [Serializable]
    public class PersistentTrackableData
    {
        public string objectId;
        public List<string> methods;
        public bool isTracking = false;

        public PersistentTrackableData(ITrackebleObject obj)
        {
            objectId = obj.GetID();
            isTracking = obj.IsTracking();

            methods = new List<string>();

            foreach (var attr in obj.GetTrackableAttributes())
            {
                if (attr.IsTracking) methods.Add(attr.MethodName);
            }
        }
    }
}
