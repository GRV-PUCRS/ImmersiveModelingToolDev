using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ITrackebleObject;

public class SocketObjectUIController : MonoBehaviour
{
    [SerializeField] private GameObject view;
    [SerializeField] private TextMeshProUGUI txtObjID;
    [SerializeField] private RectTransform socketAttributesParent;
    [SerializeField] private GameObject socketAttributeTemplate;

    private ITrackebleObject trackableObject;

    public void SetAttributes(ITrackebleObject trackableObject)
    {
        this.trackableObject = trackableObject;

        Utils.ClearChilds(socketAttributesParent);

        foreach (var attribute in trackableObject.GetTrackableAttributes())
        {
            SocketAttributeUITemplate controller = Instantiate(socketAttributeTemplate, socketAttributesParent).GetComponent<SocketAttributeUITemplate>();

            controller.SetInfo(this, attribute.MethodName, attribute.ArgsType, attribute.IsTracking);
        }
    }

    public void SetObjectID()
    {
        KeyboardManager.Instance.GetInput(SetObjectID, null, txtObjID.text);
    }

    public void SetObjectID(string newID)
    {
        trackableObject.SetID(newID);
        txtObjID.text = newID;
    }

    public void SetAttributeActive(string methodName, bool isActive) => trackableObject.SetTrackingAttribute(methodName, isActive);

    public void SetView(bool isActive)
    {
        view.SetActive(isActive);
    }

    public void DeleteSocketObject()
    {
        trackableObject.StopTracking();
    }
}
