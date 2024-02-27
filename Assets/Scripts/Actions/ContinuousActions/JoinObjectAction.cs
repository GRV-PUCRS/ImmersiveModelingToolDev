using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinObjectAction : AbstractProlongedAction
{
    private Transform tempSceneElement;

    public override void ApplyActionOnTriggerEnter(DragUI element)
    {
        if (tempSceneElement == null)
        {
            tempSceneElement = new GameObject("SceneElement").transform;
        }

        if (OculusManager.Instance.SelectionList.Contains(element.TransformToUpdate.gameObject))
        {
            GameObject[] currentSelection = new GameObject[OculusManager.Instance.SelectionList.Count];
            OculusManager.Instance.SelectionList.CopyTo(currentSelection);
            OculusManager.Instance.ClearSelection();

            foreach (GameObject sceneElement in currentSelection)
            {
                foreach(DragUI childElement in sceneElement.GetComponentsInChildren<DragUI>())
                {
                    AddElementToSceneElement(childElement);

                    Outline childDutline = childElement.GetComponent<Outline>();

                    childDutline.OutlineColor = Color.white;
                    childDutline.EnableOutline();
                }
            }

            SoundManager.Instance.PlaySound(SoundManager.Instance.disjoin);

            Debug.Log($"2 IF   {element.name}");
            return;

        }
        
        if (OculusManager.Instance.SelectionList.Count != 0)
        {
            OculusManager.Instance.ClearSelection();
        }

        if (element.TransformToUpdate == tempSceneElement)
        {
            element.SetSelected(false);

            RemoveElementFromSceneElement(element);

            SoundManager.Instance.PlaySound(SoundManager.Instance.deselection);

            Debug.Log($"InGroup Element {element.name}");
        }
        else/* if (element.TransformToUpdate != tempSceneElement)*/
        {
            element.SetSelected(true);

            Transform oldParent = element.TransformToUpdate;

            AddElementToSceneElement(element);

            if (oldParent.childCount == 0)
            {
                Destroy(oldParent.gameObject);
            }

            EventManager.TriggerJoinedObject(element, oldParent);

            SoundManager.Instance.PlaySound(SoundManager.Instance.disjoin);

            Debug.Log($"Add to Group Element {element.name}");
        }

    }

    public override void ApplyActionOnTriggerExit(DragUI element)
    {
        return;
    }

    public override void ApplyActionOnRelease()
    {
        if (tempSceneElement == null) return;

        if (tempSceneElement.childCount != 0)
        {
            /*
            foreach (Outline outline in tempSceneElement.GetComponentsInChildren<Outline>())
            {
                outline.DisableOutline();
            }

            foreach (DragUI element in tempSceneElement.GetComponentsInChildren<DragUI>())
            {
                element.IsSelected = false;
            }
            */

            foreach (DragUI element in tempSceneElement.GetComponentsInChildren<DragUI>())
            {
                element.SetSelected(false);
            }

            Utils.UpdateSceneElementPivot(tempSceneElement);

            OculusManager.Instance.TaskManager.AddObjectInTask(tempSceneElement);
        }

        tempSceneElement = null;
    }

    public override void ReleaseActionObject()
    {
        ApplyActionOnRelease();
    }

    private void AddElementToSceneElement(DragUI element)
    {
        element.transform.SetParent(tempSceneElement);
        element.SetNewTransform(tempSceneElement);
    }

    private void RemoveElementFromSceneElement(DragUI element)
    {
        Transform newSceneElement = new GameObject("SceneElement").transform;
        element.transform.SetParent(newSceneElement);
        element.SetNewTransform(newSceneElement);

        OculusManager.Instance.TaskManager.AddObjectInTask(newSceneElement);
    }
}
