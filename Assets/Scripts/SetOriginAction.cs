using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOriginAction : MonoBehaviour, IAction
{
    private bool isActive = false;
    private bool colliding = false;

    private DragUI objectToTransform;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colidiu com " + other.name);
        if (!isActive) return;

        objectToTransform = other.GetComponent<DragUI>();
        colliding = objectToTransform != null;

        Debug.Log("Salvou " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Saiu de " + other.name);
        if (!isActive) return;

        colliding = false;
        objectToTransform = null;

        Debug.Log("Resetou memoria");
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

        SetOrigin();

        isActive = false;
    }

    public void SetOrigin()
    {
        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            OculusManager.Instance.SetObjectAsOrigin(objectToTransform);
            ObjectStore.Instance.RetrieveObjectToStore(transform);
        }

        colliding = false;
        objectToTransform = null;
    }

    private void OnObjectDragBegin(Transform obj)
    {
        Debug.Log(obj.name + " ------- " + transform.name);
        if (obj != transform) return;

        isActive = true;
    }
}
