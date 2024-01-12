using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepElementTemplateController : MonoBehaviour
{
    [SerializeField] private Image _enableImage;
    [SerializeField] private TextMeshProUGUI _txtStepNumber;

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

    public void Setup(bool isReference, int stepNumber)
    {
        _isReference = isReference;
        _enableImage.color = isReference ? new Color(0f, 1f, 0.7f) : Color.green;

        _txtStepNumber.text = stepNumber.ToString();
    }
}
