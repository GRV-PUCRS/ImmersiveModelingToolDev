using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDimensionsAction : AbstractSimpleAction
{
    [SerializeField] private BlockBuilder _blockBuilderPrefab;
    
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);

        if (sceneElements.Count != 1) return;

        if (sceneElements[0].transform.childCount != 1) return;

        var element = sceneElements[0].transform.GetChild(0).GetComponentInChildren<DragUI>();

        var blockInstance = Instantiate(_blockBuilderPrefab);
        blockInstance.SetObject(element.transform);

        var hmdToObjectDirection = InputController.Instance.HMD.position - element.transform.position;
        blockInstance.transform.position = element.transform.position + hmdToObjectDirection * 0.2f;
        blockInstance.transform.rotation = Quaternion.LookRotation(-hmdToObjectDirection);
    }
}
