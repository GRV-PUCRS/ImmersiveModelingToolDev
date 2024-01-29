using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInteractionAction : AbstractSimpleAction
{
    protected override void ValidateAndApplyAction(List<GameObject> sceneElement)
    {
        ApplyAction(sceneElement);
    }

    public override void ApplyAction(List<GameObject> sceneElements)
    {
        bool toApplyAction = CheckIfAllPropertyAreFalse(sceneElements, (DragUI element) => element.IsInteractionDisabled);

        foreach (GameObject sceneElement in sceneElements)
        {
            foreach (DragUI element in sceneElement.GetComponentsInChildren<DragUI>())
            {
                element.IsInteractionDisabled = toApplyAction;

                if (toApplyAction)
                {
                    if (element.TryGetComponent<Outline>(out var outline)) outline.DisableOutline();
                }
            }
        }

        if (toApplyAction)
        {
            OculusManager.Instance.ClearSelection();
        }

        SoundManager.Instance.PlaySound(toApplyAction ? SoundManager.Instance.confirmOrigin : SoundManager.Instance.resetOrigin);
    }

    protected override void ValidadeAndOnTriggerEnterWithObject(Collider other)
    {
        base.OnTriggerEnterWithObject(other);
    }

    protected override void ValidadeAndOnTriggerExitWithObject(Collider other)
    {
        base.OnTriggerExitWithObject(other);
    }
}