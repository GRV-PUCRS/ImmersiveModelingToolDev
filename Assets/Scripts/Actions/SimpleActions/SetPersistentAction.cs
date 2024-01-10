using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPersistentAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        bool toApplyAction = CheckIfAllPropertyAreFalse(sceneElements, (DragUI element) => element.IsPersistent);

        foreach (GameObject sceneElement in sceneElements)
        {
            foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
            {
                element.IsPersistent = toApplyAction;
            }

            if (toApplyAction)
            {
                OculusManager.Instance.SetPersistentObject(sceneElement);
            }
            else
            {
                OculusManager.Instance.RemovePersistentObject(sceneElement);
            }
        }

        SoundManager.Instance.PlaySound(toApplyAction ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
    }
}