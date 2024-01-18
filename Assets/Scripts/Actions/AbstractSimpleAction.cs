using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSimpleAction : AbstractAction
{
    protected DragUI currentElement;
    private bool objInCurrentSelection = false;

    public override void OnTriggerEnterWithObject(Collider other)
    {
        DragUI obj = other.GetComponent<DragUI>();

        if (obj == null) return;

        currentElement = obj;

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
    }

    public override void OnTriggerExitWithObject(Collider other)
    {
        if (currentElement == null) return;

        if (!objInCurrentSelection)
        {
            OculusManager.Instance.RmvSelectedObject(currentElement.TransformToUpdate.gameObject);
        }

        currentElement = null;
    }

    public override void ReleaseActionObject()
    {
        if (currentElement == null) return;

        GameObject[] sceneElements = new GameObject[OculusManager.Instance.SelectionList.Count];

        OculusManager.Instance.SelectionList.CopyTo(sceneElements);
        //OculusManager.Instance.ClearSelection();

        if (sceneElements.Length != 0)
        {
            ApplyAction(new List<GameObject>(sceneElements));
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

    public abstract void ApplyAction(List<GameObject> sceneElement);
    public override void OnInstantiated()
    {
        return;
    }
}
