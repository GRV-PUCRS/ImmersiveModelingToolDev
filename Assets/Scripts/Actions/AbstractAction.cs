using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class AbstractAction : MonoBehaviour, IAction
{
    protected bool isActive = false;
    private ObjectSelector _controller;
    private Vector3 _oldScale;

    Coroutine _positionAnimationCoroutine;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (!isActive) return;
    }

    protected virtual void OnRayEnter(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        ValidadeAndOnTriggerEnterWithObject(other);
    }

    protected virtual void OnRayExit(Collider other)
    {
        if (!isActive || !other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        ValidadeAndOnTriggerExitWithObject(other);
    }

    private void OnStageChange()
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);
    }

    private void OnObjectDragEnd(ObjectSelector controller, DragUI obj)
    {
        if (obj.transform != transform || !isActive) return;

        if (_positionAnimationCoroutine != null) StopCoroutine(_positionAnimationCoroutine);

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
        if (obj.transform != transform || isActive) return;

        _controller = controller;
        _controller.OnObjectPointedEvent.AddListener(OnObjectPointedEvent);
        //_controller.HighlightColor = Color.yellow;
        _controller.WithActionHolder = true;

        _oldScale = transform.localScale;
        transform.localScale = Vector3.one * 0.55f;

        Quaternion newRotation = Quaternion.LookRotation(InputController.Instance.HMD.position - transform.position, Vector3.up);
        _positionAnimationCoroutine = StartCoroutine(SetActionPosition(transform, _controller.ActionIconPlace.position, newRotation));

        isActive = true;
    }

    private IEnumerator SetActionPosition(Transform action, Vector3 newPosition, Quaternion newOrientation, float animationTime=0.2f)
    {
        float currentTme = 0;

        Vector3 originalPosition = action.position;

        while(currentTme < animationTime)
        {
            action.position = Vector3.Lerp(originalPosition, newPosition, currentTme / animationTime);
            action.rotation = Quaternion.Lerp(action.rotation, newOrientation, currentTme / animationTime);
            yield return null;

            currentTme += Time.deltaTime;
        }
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

    protected virtual void ValidadeAndOnTriggerEnterWithObject(Collider other)
    {
        Debug.Log($"Validate enter {other.name}");
        if (other.TryGetComponent<DragUI>(out var element))
        {
            if (element.IsInteractionDisabled) return;
        }

        OnTriggerEnterWithObject(other);
    }

    protected virtual void ValidadeAndOnTriggerExitWithObject(Collider other)
    {
        Debug.Log($"Validate exit {other.name}");
        if (other.TryGetComponent<DragUI>(out var element))
        {
            if (element.IsInteractionDisabled) return;
        }

        OnTriggerExitWithObject(other);
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

    public bool IsActive()
    {
        return isActive;
    }
}
