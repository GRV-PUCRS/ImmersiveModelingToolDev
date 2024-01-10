using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IKeyboard
{
    void GetInput(InputField inputField, Action<string> onConfirm, TouchScreenKeyboardType keyboardType);
    public void SelectInputField();
    public void Confirm();
    public void CancelSelection();
}
