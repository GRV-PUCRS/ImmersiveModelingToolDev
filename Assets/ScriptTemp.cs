using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptTemp : MonoBehaviour
{
    private void Awake()
    {
        InputController.Instance.OnLeftHandTriggerUp.AddListener(Action);
        InputController.Instance.OnThreeButtonPressed.AddListener(Action2);
    }

    public void Action2()
    {

    }

    public void Action()
    {
        KeyboardManager.Instance.GetInput(null, null, "Teste");
    }
}
