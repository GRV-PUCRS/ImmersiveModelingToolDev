using UnityEngine;

public class SelectionBoxAction : AbstractProlongedAction
{
    public override void ApplyActionOnTriggerEnter(DragUI element)
    {
        GameObject sceneElement = element.TransformToUpdate.gameObject;

        if (OculusManager.Instance.SelectionList.Contains(sceneElement))
        {
            OculusManager.Instance.RmvSelectedObject(sceneElement, Color.red);
            SoundManager.Instance.PlaySound(SoundManager.Instance.deselection);
        }
        else
        {
            OculusManager.Instance.AddSelectedObject(sceneElement, Color.red);
            SoundManager.Instance.PlaySound(SoundManager.Instance.disjoin);
        }
    }

    public override void ApplyActionOnTriggerExit(DragUI element)
    {
        return;
    }

    public override void ApplyActionOnRelease()
    {
        return;
    }

    public override void ReleaseActionObject()
    {
        return;
    }
}