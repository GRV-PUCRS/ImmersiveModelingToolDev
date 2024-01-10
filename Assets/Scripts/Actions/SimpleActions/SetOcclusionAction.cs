using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOcclusionAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        bool toApplyAction = CheckIfAllPropertyAreFalse(sceneElements, (DragUI element) => element.IsOcclusion);

        Debug.Log($"[SetOcclusionAction][ApplyAction] Apply occlusion ({toApplyAction}) in {sceneElements.Count} Scene Elements");

        foreach (GameObject sceneElement in sceneElements)
        {
            foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
            {
                element.IsOcclusion = toApplyAction;
                element.IsFixed = toApplyAction;
            }
        }

        SoundManager.Instance.PlaySound(toApplyAction ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
    }
}
