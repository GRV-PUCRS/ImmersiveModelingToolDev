using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class InputController : Singleton<InputController>
{
    [Header("Buttons")]
    public UnityEvent OnStartButtonPressed;
    public UnityEvent OnOneButtonPressed;
    public UnityEvent OnTwoButtonPressed;
    public UnityEvent OnThreeButtonPressed;
    public UnityEvent OnFourButtonPressed;

    public UnityEvent OnRightHandTriggerUp;
    public UnityEvent OnRightHandTriggerDown;
    public UnityEvent OnRightIndexTriggerUp;
    public UnityEvent OnRightIndexTriggerDown;

    public UnityEvent OnLeftHandTriggerUp;
    public UnityEvent OnLeftHandTriggerDown;
    public UnityEvent OnLeftIndexTriggerUp;
    public UnityEvent OnLeftIndexTriggerDown;

    [Header("Axis")]
    public UnityEvent<Vector2> OnRightAxis2DChanged;

    [Header("Controllers")]
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    [Header("Head-Mount Display")]
    [SerializeField] private Transform hmd;

    private bool isControllerActive = false;

    void Update()
    {
        if (isControllerActive) ControllerInput();
        else HandInput();
    }


    public void ControllerInput()
    {
        Vector2 thumbstickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (thumbstickMovement != Vector2.zero)
        {
            OnRightAxis2DChanged?.Invoke(thumbstickMovement);
        }

        if (OVRInput.GetUp(OVRInput.Button.Start)) OnStartButtonPressed?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.One)) OnOneButtonPressed?.Invoke();

        if (OVRInput.GetUp(OVRInput.Button.Two)) OnTwoButtonPressed?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.Three)) OnThreeButtonPressed?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.Four)) OnFourButtonPressed?.Invoke();

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger)) OnRightHandTriggerUp?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) OnRightIndexTriggerUp?.Invoke();

        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) OnRightHandTriggerDown?.Invoke();
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) OnRightIndexTriggerDown?.Invoke();

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger)) OnLeftHandTriggerUp?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) OnLeftIndexTriggerUp?.Invoke();

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)) OnLeftHandTriggerDown?.Invoke();
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) OnLeftIndexTriggerDown?.Invoke();
    }

    private bool isPinchLeftActive = false;
    private bool isPinchRightActive = false;

    public void HandInput()
    {
        if (OVRInput.Get(OVRInput.Button.One) != isPinchRightActive)
        {
            isPinchRightActive = !isPinchRightActive;

            if (isPinchRightActive) OnRightIndexTriggerDown?.Invoke();
            else OnRightIndexTriggerUp?.Invoke();
        }

        if (OVRInput.Get(OVRInput.Button.Three) != isPinchLeftActive)
        {
            isPinchLeftActive = !isPinchLeftActive;

            if (isPinchLeftActive) OnLeftIndexTriggerDown?.Invoke();
            else OnLeftIndexTriggerUp?.Invoke();
        }
    }

    public void SetControllerActive(bool value)
    {
        isControllerActive = value;
    }

    public Vector2 GetStickState() { return OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick); }

    public Transform LeftController { get => leftController; }
    public Transform RightController { get => rightController; }
    public Transform HMD { get => hmd; }
}
