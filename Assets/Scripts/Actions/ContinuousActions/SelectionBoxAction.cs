using UnityEngine;

public class SelectionBoxAction : AbstractProlongedAction
{
    [SerializeField] private float _timeUntilNextEvent = 0.35f;

    private float _lastTime = 0f;
    private DragUI _lastElement;

    public override void ApplyActionOnTriggerEnter(DragUI element)
    {
        if (element.IsInteractionDisabled) return;

        float newtime = Time.realtimeSinceStartup;

        if (newtime - _lastTime < _timeUntilNextEvent && _lastElement == element) return;

        GameObject sceneElement = element.TransformToUpdate.gameObject;

        if (OculusManager.Instance.SelectionList.Contains(sceneElement))
        {
            OculusManager.Instance.RmvSelectedObject(sceneElement);
            SoundManager.Instance.PlaySound(SoundManager.Instance.deselection);
        }
        else
        {
            OculusManager.Instance.AddSelectedObject(sceneElement);
            SoundManager.Instance.PlaySound(SoundManager.Instance.disjoin);
        }

        _lastTime = newtime;
        _lastElement = element;
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