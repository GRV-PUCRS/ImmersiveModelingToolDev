using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSimpleAction : AbstractAction
{
    protected DragUI currentElement;

    public override void OnTriggerEnterWithObject(Collider other)
    {
        DragUI obj = other.GetComponent<DragUI>();

        if (obj == null) return;

        currentElement = obj;
        /*
        if (!OculusManager.Instance.SelectionList.Contains(currentElement.TransformToUpdate.gameObject))
        {
            if (OculusManager.Instance.SelectionList.Count != 0)
                OculusManager.Instance.ClearSelection();

            OculusManager.Instance.AddSelectedObject(currentElement.TransformToUpdate.gameObject, Color.yellow);
            objInCurrentSelection = false;
        }
        else
        {
            objInCurrentSelection = true;
        }
        */

        bool isSelected = OculusManager.Instance.SelectionList.Contains(currentElement.TransformToUpdate.gameObject);

        foreach (GameObject selectedObject in OculusManager.Instance.SelectionList)
        {
            if (selectedObject.TryGetComponent<DragUI>(out var element))
            {
                element.SetHighlightedByAction(isSelected);
            }
        }

        if (!isSelected)
        {
            foreach (DragUI element in obj.TransformToUpdate.GetComponentsInChildren<DragUI>())
            {
                element.SetHighlightedByAction(true);
            }
        }
    }

    public override void OnTriggerExitWithObject(Collider other)
    {
        DragUI obj = other.GetComponent<DragUI>();

        if (obj == null) return;

        if (OculusManager.Instance.SelectionList.Contains(obj.TransformToUpdate.gameObject))
        {
            foreach (GameObject selectedObject in OculusManager.Instance.SelectionList)
            {
                if (selectedObject.TryGetComponent<DragUI>(out var element))
                {
                    element.SetHighlightedByAction(false);
                }
            }
        }
        else
        {
            foreach (DragUI element in obj.TransformToUpdate.GetComponentsInChildren<DragUI>())
            {
                element.SetHighlightedByAction(false);
            }
        }

        currentElement = null;
    }

    public override void ReleaseActionObject()
    {
        if (currentElement == null) return;

        if (OculusManager.Instance.SelectionList.Contains(currentElement.TransformToUpdate.gameObject))
        {

            GameObject[] sceneElements = new GameObject[OculusManager.Instance.SelectionList.Count];

            OculusManager.Instance.SelectionList.CopyTo(sceneElements);
            //OculusManager.Instance.ClearSelection();

            if (sceneElements.Length != 0)
            {
                ValidateAndApplyAction(new List<GameObject>(sceneElements));
            }
        }
        else
        {
            ValidateAndApplyAction(new List<GameObject>() { currentElement.TransformToUpdate.gameObject });
        }

        currentElement = null;
    }

    public bool CheckIfAllPropertyAreFalse(List<GameObject> sceneElements, Predicate<DragUI> propertyPredicate)
    {
        bool isAllFalse = true;

        foreach (GameObject sceneElement in sceneElements)
        {
            foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
            {
                if (propertyPredicate(element))
                {
                    isAllFalse = false;
                    break;
                }
            }

            if (!isAllFalse)
                break;
        }

        return isAllFalse;
    }

    protected virtual void ValidateAndApplyAction(List<GameObject> sceneElement)
    {
        if (currentElement.IsInteractionDisabled) return;

        ApplyAction(sceneElement);
    }

    public abstract void ApplyAction(List<GameObject> sceneElement);

    public override void OnInstantiated()
    {
        return;
    }
}
