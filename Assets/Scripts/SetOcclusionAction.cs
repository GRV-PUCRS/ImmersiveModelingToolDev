using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOcclusionAction : MonoBehaviour, IAction
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

        SetOcclusion();

        isActive = false;
    }

    public void SetOcclusion()
    {
        Debug.Log("Set Occlusion " + colliding + "  " + objectToTransform);
        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            OculusManager.Instance.SetOcclusionObject(objectToTransform);
            ObjectStore.Instance.RetrieveObjectToStore(transform);
        }

        colliding = false;
        objectToTransform = null;
    }

    private void OnObjectDragBegin(Transform obj)
    {
        if (obj != transform) return;

        isActive = true;
    }
}
