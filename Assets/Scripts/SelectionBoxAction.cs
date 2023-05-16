using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBoxAction : MonoBehaviour, IAction
{
    private bool isActive = false;

    private List<DragUI> instances;

    private void Awake()
    {
        instances = new List<DragUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"))) return;

        DragUI element = other.GetComponentInChildren<DragUI>();
        Outline outline = element.GetComponent<Outline>();

        if (element == null || outline == null) return;

        bool isAdding = false;

        if (instances.Contains(element))
        {
            instances.Remove(element);

            SoundManager.Instance.PlaySound(SoundManager.Instance.deselection);
        }
        else
        {
            instances.Add(element);
            isAdding = true;

            SoundManager.Instance.PlaySound(SoundManager.Instance.selection);
        }

        outline.DisableOutline();
        outline.OutlineColor = isAdding ? Color.red : Color.white;
        outline.IsActive = isAdding;
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!isActive) return;
    }


    private void OnObjectScaleEnd(Transform origin)
    {
        Debug.Log("[SelectionBoxAction][OnObjectScaleEnd] Desativa escala em todos os objetos do grupo");

        DragUI instance = origin.GetComponentInChildren<DragUI>();

        if (instance == null) return;

        if (instances.Contains(instance))
        {
            //EventManager.OnObjectScaleEnd -= OnObjectScaleEnd;

            foreach (DragUI element in instances)
            {
                if (element == instance) continue;

                element.EndScale();
                Debug.Log("Element " + element.name + " end scale");
            }

            //EventManager.OnObjectScaleEnd += OnObjectScaleEnd;

            return;
        }
    }

    private void OnObjectScaleBegin(Transform origin, Transform pivot1, Transform pivot2)
    {
        Debug.Log("[SelectionBoxAction][OnObjectScaleBegin] Ativa escala em todos os objetos do grupo");

        DragUI instance = origin.GetComponentInChildren<DragUI>();

        if (instance == null) return;

        if (instances.Contains(instance))
        {
            //EventManager.OnObjectScaleBegin -= OnObjectScaleBegin;

            foreach (DragUI element in instances)
            {
                if (element == instance) continue;

                element.BeginScale(pivot1, pivot2);
            }

            //EventManager.OnObjectScaleBegin += OnObjectScaleBegin;

            return;
        }
    }

    private void OnStageChange()
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);

        ClearSelection();
    }

    private void ClearSelection()
    {
        foreach (DragUI instance in instances)
        {
            if (instance == null) continue;

            Outline outline = instance.GetComponent<Outline>();

            if (outline == null) continue;

            outline.DisableOutline();
            outline.OutlineColor = Color.white;
        }

        instances.Clear();
    }

    private void OnObjectDragEnd(Transform obj)
    {
        /*
        if (obj != transform || !isActive) return;

        ApplyAction();
        */

        DragUI instance = obj.GetComponentInChildren<DragUI>();

        if (instances.Contains(instance))
        {
            EventManager.OnObjectDragEnd -= OnObjectDragEnd;

            foreach (DragUI element in instances)
            {
                if (element == instance) continue;

                element.EndDrag();
            }

            EventManager.OnObjectDragEnd += OnObjectDragEnd;

            return;
        }

        if (obj != transform) return;

        isActive = false;
    }
    /*
    public void ApplyAction()
    {
        if (colliding && objectToTransform.gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject")))
        {
            //objectToTransform.IsPersistent = !objectToTransform.IsPersistent;
            OculusManager.Instance.SetPersistentObject(objectToTransform);

            SoundManager.Instance.PlaySound(objectToTransform.IsFixed ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
        }

        ObjectStore.Instance.RetrieveObjectToStore(transform);

        colliding = false;
        objectToTransform = null;
    }
    */
    private void OnObjectDragBegin(Transform obj)
    {
        //if (obj != transform) return;

        DragUI instance = obj.GetComponentInChildren<DragUI>();

        if (instances.Contains(instance))
        {
            EventManager.OnObjectDragBegin -= OnObjectDragBegin;

            foreach (DragUI element in instances)
            {
                if (element == instance) continue;

                element.BeginDrag(instance.TransformToUpdate.parent);
            }

            EventManager.OnObjectDragBegin += OnObjectDragBegin;

            return;
        }

        if (obj != transform) return;

        isActive = true;
    }

    private void OnObjectDuplicated(DragUI obj)
    {
        if (!instances.Contains(obj)) return;


        EventManager.OnObjectDuplicated -= OnObjectDuplicated;

        GameObject tempObject = new GameObject();
        DuplicateObjectAction action = tempObject.AddComponent<DuplicateObjectAction>();

        foreach (DragUI instance in instances)
        {
            if (obj == instance) continue;

            action.ApplyAction(instance);
        }

        foreach (DragUI instance in instances)
        {
            Outline outline = instance.GetComponent<Outline>();

            if (outline == null) return;

            outline.EnableOutline();
        }

        EventManager.OnObjectDuplicated += OnObjectDuplicated;
    }

    private void OnObjectSetPersistent(DragUI obj)
    {
        if (!instances.Contains(obj)) return;

        EventManager.OnObjectSetPersistent -= OnObjectSetPersistent;

        GameObject tempObject = new GameObject();
        SetPersistentAction action = tempObject.AddComponent<SetPersistentAction>();

        foreach (DragUI instance in instances)
        {
            if (obj == instance) continue;

            action.ApplyAction(instance);
            Debug.Log("Persistent to " + instance.name);
        }

        EventManager.OnObjectSetPersistent += OnObjectSetPersistent;
    }

    private void OnEnable()
    {
        EventManager.OnObjectDragBegin += OnObjectDragBegin;
        EventManager.OnObjectDragEnd += OnObjectDragEnd;

        EventManager.OnObjectScaleBegin += OnObjectScaleBegin;
        EventManager.OnObjectScaleEnd += OnObjectScaleEnd;

        EventManager.OnObjectDuplicated += OnObjectDuplicated;
        EventManager.OnObjectSetPersistent += OnObjectSetPersistent;

        EventManager.OnStageChange += OnStageChange;
    }

    private void OnDisable()
    {
        EventManager.OnObjectDragBegin -= OnObjectDragBegin;
        EventManager.OnObjectDragEnd -= OnObjectDragEnd;

        EventManager.OnObjectScaleBegin -= OnObjectScaleBegin;
        EventManager.OnObjectScaleEnd -= OnObjectScaleEnd;

        EventManager.OnObjectDuplicated -= OnObjectDuplicated;
        EventManager.OnObjectSetPersistent -= OnObjectSetPersistent;

        EventManager.OnStageChange -= OnStageChange;
    }
}