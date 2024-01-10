using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NativeKeyboard : MonoBehaviour, IKeyboard
{
    [SerializeField] private GameObject keyboardView;
    [SerializeField] private InputField inputField;

    private TouchScreenKeyboard overlayKeyboard;
    private bool isActive = false;
    private Action<string> onConfirm;


    private void Update()
    {
        if (isActive)
        {
            if (overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                Confirm();
                isActive = false;

            }

            if (overlayKeyboard.text != inputField.text)
            {
                inputField.text = overlayKeyboard.text;
            }
        }

    }

    public void GetInput(InputField inputField, Action<string> onConfirm, TouchScreenKeyboardType touchScreenKeyboardType)
    {
        //keyboardView.SetActive(true);

        this.onConfirm = onConfirm;
        this.inputField = inputField;

        // Posiciona teclado na frente do usuario
        //keyboardView.transform.position = InputController.Instance.RightController.position + InputController.Instance.HMD.forward * 0.5f;
        //keyboardView.transform.rotation = Quaternion.Euler(InputController.Instance.HMD.rotation.eulerAngles + new Vector3(-90, 0, 0));

        overlayKeyboard = TouchScreenKeyboard.Open(inputField.text, touchScreenKeyboardType);
        isActive = true;
    }

    public void SelectInputField()
    {
        inputField.Select();
    }
    public void CancelSelection()
    {
        keyboardView.SetActive(false);
    }

    public void Confirm()
    {
        inputField.text = overlayKeyboard.text;
        onConfirm?.Invoke(inputField.text);

        //keyboardView.SetActive(false);
    }
}
