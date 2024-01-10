using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTrackableObjectAction : AbstractSimpleAction
{
    public override void ApplyAction(List<GameObject> sceneElement)
    {
        if (sceneElement.Count != 1 || sceneElement[0].transform.childCount != 1)
        {
            Debug.Log("[AddTrackableObjectAction] Scenelement child Count != 1");
            return;
        }

        Transform obj = sceneElement[0].transform.GetChild(0);

        ITrackebleObject trackableObject = sceneElement[0].GetComponentInChildren<ITrackebleObject>();

        if (trackableObject.IsTracking())
        {
            Debug.Log("[AddTrackableObjectAction] Turn on Trackable Object UI");
            trackableObject.SetActiveUI(true);
        }
        else
        {
            Debug.Log("[AddTrackableObjectAction] Start tracking object");
            trackableObject.StartTracking();
        }

        Transform uiTransform = trackableObject.GetUITransform();
        uiTransform.SetParent(sceneElement[0].transform.parent);
        uiTransform.localScale = Vector3.one;

        Bounds bounds = Utils.GetBounds(obj.gameObject);

        uiTransform.transform.position = obj.transform.position + Vector3.right * bounds.extents.x;
        uiTransform.LookAt(InputController.Instance.HMD);
    }
}
