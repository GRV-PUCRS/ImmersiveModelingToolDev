using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFixedAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        bool toApplyAction = CheckIfAllPropertyAreFalse(sceneElements, (DragUI element) => element.IsFixed);

        foreach (GameObject sceneElement in sceneElements)
        {
            foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
            {
                element.IsFixed = toApplyAction;
            }
        }

        SoundManager.Instance.PlaySound(toApplyAction ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
    }
}