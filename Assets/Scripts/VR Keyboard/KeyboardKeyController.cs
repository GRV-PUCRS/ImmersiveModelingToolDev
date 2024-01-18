using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardKeyController : MonoBehaviour
{
    public KeyCode _keyCode;
    [SerializeField] private TextMeshProUGUI _txtKeyCode;
    [SerializeField] private bool _onValidateOn = true;

    private void OnValidate()
    {
        if (!_onValidateOn) return;

        _txtKeyCode.text = _keyCode.ToString();
    }
}
