using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProlongedAction : AbstractAction
{
    public override void OnTriggerEnterWithObject(Collider other)
    {
        DragUI element = other.GetComponentInChildren<DragUI>();

        if (element == null) return;

        ApplyActionOnTriggerEnter(element);
    }

    public override void OnTriggerExitWithObject(Collider other)
    {
        DragUI element = other.GetComponentInChildren<DragUI>();

        if (element == null) return;

        ApplyActionOnTriggerExit(element);
    }

    public abstract void ApplyActionOnTriggerEnter(DragUI element);
    public abstract void ApplyActionOnTriggerExit(DragUI element);
    public abstract void ApplyActionOnRelease();
    public override void OnInstantiated()
    {
        return;
    }
}
