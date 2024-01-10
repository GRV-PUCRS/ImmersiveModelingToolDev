using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateObjectAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        foreach (GameObject sceneElement in sceneElements)
        {
            Debug.Log($"[DuplicateObjectAction][Action] Duplacating object {sceneElement.name}");

            GameObject duplicatedObj = Instantiate(sceneElement);

            DragUI[] childs = duplicatedObj.GetComponentsInChildren<DragUI>();

            foreach (DragUI child in childs)
            {
                child.transform.SetParent(duplicatedObj.transform);
                child.SetNewTransform(duplicatedObj.transform);

                child.IsFixed = false;
                child.IsReferenceObject = false;
                child.IsOcclusion = false;

                if (child.TryGetComponent<Outline>(out var outline)) outline.DisableOutline();
            }

            OculusManager.Instance.AddObjectToTask(duplicatedObj.transform);

            duplicatedObj.transform.localPosition = sceneElement.transform.localPosition + new Vector3(0.1f, 0, 0);
            duplicatedObj.transform.localRotation = sceneElement.transform.localRotation;
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);
    }
}