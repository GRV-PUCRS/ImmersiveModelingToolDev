using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectTrackable : AbstractTrackableObject
{
    private MeshRenderer meshRenderer;

    private List<ITrackebleObject.TrackableAttribute> attributes = new List<ITrackebleObject.TrackableAttribute>()
    {
        new ITrackebleObject.TrackableAttribute("SetPosition", new List<string>() { "float", "float", "float" }, false),
        new ITrackebleObject.TrackableAttribute("SetRotation", new List<string>() { "float", "float", "float" }, false),
        new ITrackebleObject.TrackableAttribute("SetScale", new List<string>() { "float", "float", "float" }, false),
        new ITrackebleObject.TrackableAttribute("SetColor", new List<string>() { "float", "float", "float" }, false),
        new ITrackebleObject.TrackableAttribute("SetActive", new List<string>() { "bool" }, false)
    };

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }


    public override List<ITrackebleObject.TrackableAttribute> GetTrackableAttributes() => attributes;

    public override void TriggerMethod(string methodName, object[] args)
    {
        switch (methodName)
        {
            case "SetPosition": SetPosition(args); break;
            case "SetRotation": SetRotation(args); break;
            case "SetScale": SetScale(args); break;
            case "SetColor": SetColor(args); break;
            case "SetActive": SetActive(args); break;
            default: break;
        }
    }

    public override DragUI GetObjectController()
    {
        return GetComponentInChildren<DragUI>();
    }

    public void SetActive(object[] args)
    {
        bool isActive = (bool)args[0];

        gameObject.SetActive(isActive);
    }

    public void SetColor(object[] args)
    {
        Vector3 color = ObjectToVector3(args);

        meshRenderer.material.color = new Color(color.x, color.y, color.z);
    }

    public void SetRotation(object[] args)
    {
        transform.eulerAngles = ObjectToVector3(args);
    }

    public void SetPosition(object[] args)
    {
        transform.position = ObjectToVector3(args);
    }

    public void SetScale(object[] args)
    {
        transform.localScale = ObjectToVector3(args);
    }

    private Vector3 ObjectToVector3(object[] args)
    {
        if (args.Length != 3) return Vector3.zero;

        return new Vector3((float)args[0], (float)args[1], (float)args[2]);
    }
}
