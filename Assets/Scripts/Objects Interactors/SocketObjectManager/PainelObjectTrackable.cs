using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainelObjectTrackable : AbstractTrackableObject
{
    private const string SET_TEXT_PERMANENT_EVENT = "SetTextPermanently";
    private const string SET_TEXT_TEMPORARY_EVENT = "SetTextTemporarily";

    private PainelController painelController;

    List<ITrackebleObject.TrackableAttribute> attributes = new List<ITrackebleObject.TrackableAttribute>()
    {
        new ITrackebleObject.TrackableAttribute(SET_TEXT_PERMANENT_EVENT, new List<string>() { "string" }, false),
        new ITrackebleObject.TrackableAttribute(SET_TEXT_TEMPORARY_EVENT, new List<string>() { "string" }, false)
    };

    private void Start()
    {
        painelController = GetComponentInChildren<PainelController>();
    }

    public override List<ITrackebleObject.TrackableAttribute> GetTrackableAttributes() => attributes;

    public override void TriggerMethod(string methodName, object[] args)
    {
        switch (methodName)
        {
            case SET_TEXT_PERMANENT_EVENT: SetText(args, true); break;
            case SET_TEXT_TEMPORARY_EVENT: SetText(args, false); break;
            default: break;
        }
    }

    public override DragUI GetObjectController()
    {
        return GetComponentInChildren<DragUI>();
    }

    public void SetText(object[] args, bool isPermanent)
    {
        string newText = (string) args[0];

        painelController.SetTextPainel(newText, isPermanent);
    }
}
