using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorAction : AbstractSimpleAction
{
    private ColorController colorController;
    
    public override void ApplyAction(List<GameObject> sceneElements)
    {
        ObjectStore.Instance.RetrieveObjectToStore(transform);

        if (sceneElements.Count != 1) return;

        if (sceneElements[0].transform.childCount != 1) return;

        DragUI element = sceneElements[0].transform.GetChild(0).GetComponentInChildren<DragUI>();
        colorController = element.ColorController;

        ColorPickerUI colorPickerUI = ColorPickerUI.Instance;
        colorPickerUI.SetColorAction(this);
        colorPickerUI.SetInfo(colorController.MeshRenderers);
        //colorPickerUI.SetGroup(sceneElements);

        var hmdToObjectDirection = InputController.Instance.HMD.position - element.transform.position;
        colorPickerUI.transform.position = element.transform.position + hmdToObjectDirection * 0.2f;
        colorPickerUI.transform.rotation = Quaternion.LookRotation(-hmdToObjectDirection);
    }

    public override void OnTriggerEnterWithObject(Collider other)
    {
        OculusManager.Instance.ClearSelection();
        base.OnTriggerEnterWithObject(other);
    }
    
    public void Completed()
    {

    }

    public void AddColor(int index, Color color)
    {
        colorController.AddColor(index, color);
    }

    public void RmvColor(int index)
    {
        colorController.RmvColor(index);
    }

    public void SelectComponent(int index)
    {
        colorController.SelectComponent(index);
    }

    public void UnselectComponent(int index)
    {
        colorController.UnselectComponent(index);
    }
}
