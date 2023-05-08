using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOneGrabTranslateTransformer : MonoBehaviour, ITransformer
{
    [SerializeField] private Transform controller;

    private Transform oldParent;

    private void Awake()
    {
        oldParent = transform.parent;
    }

    public void OnSelected()
    {
        Logger.Log("Selected: old " + transform.parent.name + " -> " + transform.name);
        transform.parent = controller;
        Logger.Log("Selected: new " + transform.parent.name + " -> " + transform.name);
    }

    public void OnUnselected()
    {
        transform.parent = oldParent;
        Logger.Log("Unselected: new " + transform.parent.name + " -> " + transform.name);
    }

    public void Initialize(IGrabbable grabbable)
    {

    }

    public void BeginTransform()
    {
        OnSelected();
    }

    public void UpdateTransform()
    {

    }

    public void EndTransform()
    {
        OnUnselected();
    }
}
