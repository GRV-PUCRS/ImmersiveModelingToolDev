using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectStatus
{
    DESELECTED,
    UNDETACHED,
    DETACH,
    SELECTED
}

public class ObjectController : MonoBehaviour
{
    [SerializeField] private Color detachColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private bool sendToClient = true;

    private ObjectStatus currentStatus;
    private MeshRenderer[] meshRenderes;
    private Color[] currentColors;

    private void Awake()
    {
        meshRenderes = GetComponentsInChildren<MeshRenderer>();
        currentColors = new Color[meshRenderes.Length];

        for (int i = 0; i < meshRenderes.Length; i++)
        {
            currentColors[i] = meshRenderes[i].material.color;
        }
    }

    public void SetObjectStatus(ObjectStatus newStatus)
    {
        switch (newStatus)
        {
            case ObjectStatus.DESELECTED:
                ResetObjectColor();
                break;

            case ObjectStatus.UNDETACHED:
                if (currentStatus == ObjectStatus.SELECTED) return;

                ResetObjectColor();
                break;

            case ObjectStatus.DETACH:
                if (currentStatus == ObjectStatus.SELECTED) return;

                SetObjectColor(detachColor);
                break;

            case ObjectStatus.SELECTED:
                SetObjectColor(selectedColor);
                break;
        }

        currentStatus = newStatus;
    }

    private void ResetObjectColor()
    {
        for (int i = 0; i < meshRenderes.Length; i++)
        {
            meshRenderes[i].material.color = currentColors[i];
        }
    }

    private void SetObjectColor(Color newColor)
    {
        for (int i = 0; i < meshRenderes.Length; i++)
        {
            meshRenderes[i].material.color = newColor;
        }
    }

    public void SelectObject()
    {
        if (currentStatus == ObjectStatus.SELECTED)
            SetObjectStatus(ObjectStatus.DESELECTED);
        else
            SetObjectStatus(ObjectStatus.SELECTED);

        if (sendToClient)
            EventManager.TriggerSendMessageRequest(Events.ACTION_OBJECT, new object[3] { gameObject.name, transform.position + Vector3.up * (0.2f), currentStatus == ObjectStatus.SELECTED });
    }
}
