using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFixedAction : MonoBehaviour, IAction
{
    private bool isActive = false;
    private bool colliding = false;

    private DragUI objectToTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        objectToTransform = other.GetComponent<DragUI>();
        colliding = objectToTransform != null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive) return;

        colliding = false;
        objectToTransform = null;
    }

    private void OnEnable()
    {
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;
        EventManager.OnStageChange += OnStageChange;
    }

    private void OnDisable()
    {
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;
        EventManager.OnStageChange -= OnStageChange;
    }

    private void OnStageChange()
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);
    }

    private void OnObjectDragEnd(Transform obj)
    {
        if (obj != transform || !isActive) return;

        ApplyAction();

        isActive = false;
    }

    public void ApplyAction()
    {
        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            objectToTransform.IsFixed = !objectToTransform.IsFixed;
            //OculusManager.Instance.SetFixedObject(objectToTransform);

            SoundManager.Instance.PlaySound(objectToTransform.IsFixed ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
        }

        ObjectStore.Instance.RetrieveObjectToStore(transform);

        colliding = false;
        objectToTransform = null;
    }

    private void OnObjectDragBegin(Transform obj)
    {
        if (obj != transform) return;

        isActive = true;
    }
}