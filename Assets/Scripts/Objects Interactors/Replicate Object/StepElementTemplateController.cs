using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepElementTemplateController : MonoBehaviour
{
    [SerializeField] private Image _enableImage;

    private Toggle _toggle;
    private bool _isReference;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    public void ToggleIsOn()
    {
        if (_isReference) return;

        _toggle.ToggleIsOn();
    }

    public void Setup(bool isReference)
    {
        _isReference = isReference;
        _enableImage.color = isReference ? new Color(0f, 1f, 0.7f) : Color.green;
    }
}
