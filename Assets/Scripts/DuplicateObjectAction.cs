using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateObjectAction : MonoBehaviour, IAction
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
        Debug.Log("Set Occlusion " + colliding + "  " + objectToTransform);

        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            GameObject duplicatedObj = Instantiate(objectToTransform.TransformToUpdate.gameObject);
            OculusManager.Instance.AddObjectToTask(duplicatedObj.transform);

            duplicatedObj.name = objectToTransform.name;
            duplicatedObj.transform.localPosition = objectToTransform.TransformToUpdate.localPosition + new Vector3(0.1f, 0.1f, 0);

            duplicatedObj.GetComponentInChildren<DragUI>().IsFixed = false;
            duplicatedObj.GetComponentInChildren<DragUI>().IsReferenceObject = false;

            if (duplicatedObj.GetComponentInChildren<OcclusionObject>() != null)
                OculusManager.Instance.SetOcclusionObject(duplicatedObj.GetComponentInChildren<DragUI>());

            SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);

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
