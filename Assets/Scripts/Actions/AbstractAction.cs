using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class AbstractAction : MonoBehaviour, IAction
{
    protected bool isActive = false;
    private ObjectSelector _controller;
    private Vector3 _oldScale;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnRayEnter(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        OnTriggerEnterWithObject(other);
    }

    private void OnRayExit(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        OnTriggerExitWithObject(other);
    }

    private void OnStageChange()
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);
    }

    private void OnObjectDragEnd(ObjectSelector controller, DragUI obj)
    {
        if (obj.transform != transform || !isActive) return;

        ReleaseActionObject();

        isActive = false;
        OnStageChange();

        _controller.OnObjectPointedEvent.RemoveListener(OnObjectPointedEvent);
        //_controller.HighlightColor = Color.white;
        _controller.WithActionHolder = false;
        transform.localScale = _oldScale;

        obj.SetHighlighted(false);
    }

    private void OnObjectDragBegin(ObjectSelector controller, DragUI obj)
    {
        if (obj.transform != transform) return;

        _controller = controller;
        _controller.OnObjectPointedEvent.AddListener(OnObjectPointedEvent);
        //_controller.HighlightColor = Color.yellow;
        _controller.WithActionHolder = true;

        transform.position = _controller.ActionIconPlace.position;
        _oldScale = transform.localScale;
        transform.localScale = Vector3.one * 0.7f;

        isActive = true;
    }

    private void OnObjectPointedEvent(bool isLeft, GameObject obj, bool isHighlighted)
    {
        if (isLeft != _controller.IsLeft) return;

        var collider = obj.GetComponent<Collider>();

        if (collider == null) return;

        if (isHighlighted)
        {
            OnRayEnter(collider);
        }
        else
        {
            OnRayExit(collider);
        }
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
