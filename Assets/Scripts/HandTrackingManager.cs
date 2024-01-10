using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Oculus.Interaction;

public class HandTrackingManager : Singleton<HandTrackingManager>
{
    [SerializeField] private bool allowHandTracking = true;

    [Header("Hand References")]
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    [Header("Controllers References")]
    [SerializeField] private GameObject controllersParent;

    public UnityEvent OnThumbsUpSelected;
    public UnityEvent OnThumbsUpUnselected;

    private bool isEnable = false;

    private void Start()
    {
        SetHandActive(false);
    }

    private void Update()
    {
        if (!allowHandTracking) return;

        if (isEnable != OVRPlugin.GetHandTrackingEnabled())
        {
            isEnable = !isEnable;
            SetHandActive(isEnable);
        }
    }

    public void SetHandActive(bool newValue)
    {
        leftHand.SetActive(newValue);
        rightHand.SetActive(newValue);
        controllersParent.SetActive(!newValue);

        InputController.Instance.SetControllerActive(!newValue);
    }

    public void OnThumbsDectected(bool isSelected)
    {
        if (isSelected) OnThumbsUpSelected?.Invoke();
        else OnThumbsUpUnselected?.Invoke();
    }

    public Transform LeftHand { get => leftHand.transform; }
    public Transform RightHand { get => rightHand.transform; }
}
