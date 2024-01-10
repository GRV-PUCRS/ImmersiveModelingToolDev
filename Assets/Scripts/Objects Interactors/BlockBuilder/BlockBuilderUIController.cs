using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BlockBuilderUIController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BlockBuilder builder;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI txtXAxis;
    [SerializeField] private TextMeshProUGUI txtYAxis;
    [SerializeField] private TextMeshProUGUI txtZAxis;
    
    public void ChangeAxisValue(TextMeshProUGUI txtField)
    {
        KeyboardManager.Instance.GetInput(result => ChangeAxis(txtField, result), null, txtField.text, TouchScreenKeyboardType.NumbersAndPunctuation | TouchScreenKeyboardType.DecimalPad);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeAxis(txtXAxis, "-0,8");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeAxis(txtYAxis, "0,1");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeAxis(txtZAxis, "0,3");
        }
    }

    public void InvertSignal(TextMeshProUGUI txtField)
    {
        float value = float.Parse(txtField.text) * -1;

        txtField.text = value.ToString("0.0000");
        UpdateBlockScale();
    }

    private void ChangeAxis(TextMeshProUGUI txtAxis, string value)
    {
        if (value.Length == 0) return;

        float floatValue = float.Parse(value);

        if (floatValue == 0f) return;

        txtAxis.text = floatValue.ToString("0.0000");
        UpdateBlockScale();
    }

    private void UpdateBlockScale()
    {
        Vector3 newScale = new Vector3();

        newScale.x = float.Parse(txtXAxis.text);
        newScale.y = float.Parse(txtYAxis.text);
        newScale.z = float.Parse(txtZAxis.text);

        builder.ChangeAxis(newScale);
    }

    public void InitValues(Vector3 values)
    {
        txtXAxis.text = values.x.ToString("0.0000");
        txtYAxis.text = values.y.ToString("0.0000");
        txtZAxis.text = values.z.ToString("0.0000");
    }
}
