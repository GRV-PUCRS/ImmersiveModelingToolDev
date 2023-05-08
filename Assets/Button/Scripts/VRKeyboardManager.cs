using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;
using System.Text;

public class VRKeyboardManager : Singleton<VRKeyboardManager>
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject keyboardView;
    [SerializeField] private GameObject capsOnObject;
    [SerializeField] private GameObject capsOffObject;

    [SerializeField] private GameObject rightControllerStick;
    [SerializeField] private GameObject leftControllerStick;

    private bool confirmUserInput;
    private Coroutine currentCoroutineInstance;

    public bool CapsOn { get => capsOnObject.activeInHierarchy;
        set
        {
            capsOnObject.SetActive(value);
            capsOffObject.SetActive(!value);
        }
    }

    public void GetUserInputString(Action<string> callback, string deafultText="", TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
    {
        inputField.contentType = contentType;

        if (currentCoroutineInstance != null)
            StopCoroutine(currentCoroutineInstance);

        currentCoroutineInstance = StartCoroutine(GetStringFromKeyboard(callback, deafultText));
    }

    private IEnumerator GetStringFromKeyboard(Action<string> callback, string deafultText = "")
    {
        keyboardView.SetActive(true);
        rightControllerStick.SetActive(true);
        leftControllerStick.SetActive(true);


        inputField.text = deafultText;
        confirmUserInput = false;

        // InputField Focus
        inputField.caretPosition = deafultText.Length;
        FocusInputField();

        // Posiciona teclado na frente do usuario
        keyboardView.transform.position = InputController.Instance.RightController.position + InputController.Instance.HMD.forward * 0.5f;
        keyboardView.transform.rotation = Quaternion.Euler(InputController.Instance.HMD.rotation.eulerAngles + new Vector3(-90, 0, 0));

        yield return new WaitWhile(() => !confirmUserInput);

        callback?.Invoke(inputField.text);

        keyboardView.SetActive(false);
        rightControllerStick.SetActive(false);
        leftControllerStick.SetActive(false);

        currentCoroutineInstance = null;

        Debug.Log("TEXTO: " + inputField.text);
    }

    public void MoveCaret(bool toLeft)
    {
        inputField.caretPosition += toLeft ? -1 : 1;
        inputField.ForceLabelUpdate();
        //FocusInputField();
    }

    public void BackspaceEvent()
    {
        if (inputField.text.Length == 0) return;

        int oldCaretPosition = inputField.caretPosition;
        StringBuilder oldString = new StringBuilder(inputField.text);

        oldString.Remove(oldCaretPosition - 1, 1);

        inputField.text = oldString.ToString();
        inputField.caretPosition = oldCaretPosition - 1;
        inputField.ForceLabelUpdate();

        //FocusInputField();
    }

    public void Confirm()
    {
        confirmUserInput = true;
    }

    public void FocusInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void AddCharacter(string character)
    {
        string oldString = inputField.text.Substring(0, inputField.caretPosition);
        oldString += character;

        inputField.text = oldString + inputField.text.Substring(inputField.caretPosition, inputField.text.Length - inputField.caretPosition);
        inputField.caretPosition += 1;

        inputField.ForceLabelUpdate();
    }
}
