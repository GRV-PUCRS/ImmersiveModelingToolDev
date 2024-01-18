using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardManager : Singleton<KeyboardManager>
{
    [SerializeField] private GameObject activeKeyboardMethod;
    [SerializeField] private ObjectSelector _controller;

    [SerializeField] private InputField inputField;
    [SerializeField] private UIButton btnCancel;
    [SerializeField] private UIButton btnConfirm;

    private IKeyboard keyboard = null;

    private void Awake()
    {
        if (activeKeyboardMethod == null) return;

        keyboard = activeKeyboardMethod.GetComponentInChildren<IKeyboard>();
    }

    // Reference: https://forum.unity.com/threads/howto-inputfield-always-show-caret.424556/
    public void GetInput(Action<string> onConfirm, Action onCancel, string defaultString = "", TouchScreenKeyboardType touchScreenKeyboardType = TouchScreenKeyboardType.Default)
    {
        if (keyboard == null)
        {
            Debug.Log("[KeyboardManager][WARNING] No keyboard method defined!");
            return;
        }

        //_controller.SetRayActive(false);
        activeKeyboardMethod.SetActive(true);

        inputField.text = defaultString;

        activeKeyboardMethod.transform.position = InputController.Instance.RightController.position + InputController.Instance.HMD.forward * 0.5f;
        var eulerAngle = InputController.Instance.HMD.rotation.eulerAngles;
        eulerAngle.z = 0;
        activeKeyboardMethod.transform.eulerAngles = eulerAngle;

        keyboard.GetInput(inputField, result => { onConfirm?.Invoke(result); _controller.SetRayActive(true); }, touchScreenKeyboardType);
    }

    public void SelectInputField()
    {
        keyboard.SelectInputField();
    }

    public void CancelSelection()
    {
        keyboard.CancelSelection();
    }
}
