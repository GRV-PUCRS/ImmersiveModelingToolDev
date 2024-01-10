using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOriginAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        bool currentPropertyStatus = CheckIfAllPropertyAreFalse(sceneElements, (DragUI element) => element.IsFixed);

        List<DragUI> elements = new List<DragUI>();

        foreach (GameObject sceneElement in sceneElements)
        {
            elements.AddRange(sceneElement.GetComponentsInChildren<DragUI>());
        }

        if (currentPropertyStatus)
        {
            foreach (DragUI element in elements)
            {
                element.transform.SetParent(currentElement.transform);
            }


            currentElement.transform.localPosition = Vector3.zero;
            currentElement.transform.localRotation = Quaternion.identity;

            currentElement.TransformToUpdate.localPosition = Vector3.zero;
            currentElement.TransformToUpdate.localRotation = Quaternion.identity;

            foreach (DragUI element in elements)
            {
                element.transform.SetParent(element.TransformToUpdate);
            }

            SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.resetOrigin);
        }

        foreach (DragUI element in elements)
        {
            element.IsFixed = currentPropertyStatus;
        }
    }
}
