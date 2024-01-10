using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateObjectAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        foreach (GameObject sceneElement in sceneElements)
        {
            OculusManager.Instance.ReplicateObject(sceneElement);
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.confirmOrigin);
    }
}
