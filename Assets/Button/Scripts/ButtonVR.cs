/**************************************************
Copyright : Copyright (c) RealaryVR. All rights reserved.
Description: Script for VR Button functionality.
***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ButtonVR : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;

    void Start()
    {
        if (transform.parent.name.Equals("Confirm"))
        {
            onRelease.AddListener(UpdateInputField);
        }
        else
        {
            onPress.AddListener(UpdateInputField);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnButtonPressed();
    }

    private void OnTriggerExit(Collider other)
    {
        OnButtonReleased();
    }

    public void UpdateInputField()
    {
        Debug.Log(transform.parent.name);
        if (transform.parent.name.Equals("Backspace"))
        {
            /*
            string oldString = inputField.text;

            if (oldString != "") inputField.text = oldString.Substring(0, oldString.Length-1);
            */
            VRKeyboardManager.Instance.BackspaceEvent();
            
        }
        else if (transform.parent.name.Equals("Enter"))
        {
            //inputField.text += '\n';
            VRKeyboardManager.Instance.AddCharacter("\n");
        }
        else if (transform.parent.name.Equals("Space"))
        {
            //inputField.text += " ";
            VRKeyboardManager.Instance.AddCharacter(" ");
        }
        else if(transform.parent.name.Equals("Caps"))
        {
            VRKeyboardManager.Instance.CapsOn = !VRKeyboardManager.Instance.CapsOn;
        }
        else if (transform.parent.name.Equals("Confirm"))
        {
            VRKeyboardManager.Instance.Confirm();
        }else if (transform.parent.name.Equals("Left Arrow"))
        {
            VRKeyboardManager.Instance.MoveCaret(true);
        }
        else if (transform.parent.name.Equals("Right Arrow"))
        {
            VRKeyboardManager.Instance.MoveCaret(false);
        }
        else
        {
            //inputField.text += transform.parent.name;
            VRKeyboardManager.Instance.AddCharacter(transform.parent.name);
        }
    }

    public void OnButtonPressed()
    {
        button.transform.localPosition = new Vector3(0, 0.003f, 0);
        onPress.Invoke();
        SoundManager.Instance.PlaySound(SoundManager.Instance.clickKey);
    }

    public void OnButtonReleased()
    {
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        onRelease.Invoke();
    }
}
