using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class AbstractAction : MonoBehaviour, IAction
{
    protected bool isActive = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        OnTriggerEnterWithObject(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        OnTriggerExitWithObject(other);
    }

    private void OnStageChange()
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);
    }

    private void OnObjectDragEnd(DragUI obj)
    {
        if (obj.transform != transform || !isActive) return;

        ReleaseActionObject();

        isActive = false;
        OnStageChange();
    }

    private void OnObjectDragBegin(DragUI obj)
    {
        if (obj.transform != transform) return;

        isActive = true;
    }

    protected virtual void OnEnable()
    {
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        EventManager.OnStageChange += OnStageChange;
    }

    protected virtual void OnDisable()
    {
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        EventManager.OnStageChange -= OnStageChange;
    }

    public abstract void OnTriggerEnterWithObject(Collider other);
    public abstract void OnTriggerExitWithObject(Collider other);
    public abstract void ReleaseActionObject();

    public abstract void OnInstantiated();
}
