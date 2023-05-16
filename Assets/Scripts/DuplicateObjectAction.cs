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

        isActive = false;

        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            ApplyAction(objectToTransform);

            SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);
            ObjectStore.Instance.RetrieveObjectToStore(transform);

            EventManager.TriggerObjectDuplicated(objectToTransform);
        }

        colliding = false;
        objectToTransform = null;
    }

    public void ApplyAction(DragUI instance)
    {
        Debug.Log("Set Occlusion " + colliding + "  " + instance);

        Outline outline = instance.TransformToUpdate.gameObject.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.DisableOutline();
            Debug.Log("outline");
        }

        GameObject duplicatedObj = Instantiate(instance.TransformToUpdate.gameObject);
        OculusManager.Instance.AddObjectToTask(duplicatedObj.transform);

        duplicatedObj.name = instance.name;
        duplicatedObj.transform.localPosition = instance.TransformToUpdate.localPosition + new Vector3(0.1f, 0.1f, 0);

        duplicatedObj.GetComponentInChildren<DragUI>().IsFixed = false;
        duplicatedObj.GetComponentInChildren<DragUI>().IsReferenceObject = false;

        if (duplicatedObj.GetComponentInChildren<OcclusionObject>() != null)
            OculusManager.Instance.SetOcclusionObject(duplicatedObj.GetComponentInChildren<DragUI>());
    }

    private void OnObjectDragBegin(Transform obj)
    {
        if (obj != transform) return;

        isActive = true;
    }
}