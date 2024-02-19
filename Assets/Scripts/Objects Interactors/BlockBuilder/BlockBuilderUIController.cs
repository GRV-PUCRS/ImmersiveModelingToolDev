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

    private const float CURRENT_SCALE = 100f;
    private const string VALUES_FORMAT = "0.00";

    public void ChangeAxisValue(TextMeshProUGUI txtField)
    {
        KeyboardManager.Instance.GetInput(result => ChangeAxis(txtField, result), null, txtField.text, TouchScreenKeyboardType.NumbersAndPunctuation | TouchScreenKeyboardType.DecimalPad);
    }

    private void ChangeAxis(TextMeshProUGUI txtAxis, string value)
    {
        if (value.Length == 0) return;

        float floatValue = ParseFloat(value);

        if (floatValue == 0f) return;

        txtAxis.text = ParseFloat(floatValue);
        UpdateBlockScale();
    }

    private void UpdateBlockScale()
    {
        Vector3 newScale = new Vector3();

        newScale.x = ParseFloat(txtXAxis.text);
        newScale.y = ParseFloat(txtYAxis.text);
        newScale.z = ParseFloat(txtZAxis.text);

        builder.ChangeAxis(newScale / CURRENT_SCALE);
    }

    public void InitValues(Vector3 values)
    {
        Vector3 formattedValues = values * CURRENT_SCALE;

        txtXAxis.text = ParseFloat(formattedValues.x);
        txtYAxis.text = ParseFloat(formattedValues.y);
        txtZAxis.text = ParseFloat(formattedValues.z);
    }

    public string ParseFloat(float value) => value.ToString(VALUES_FORMAT).Replace(",", ".");
    public float ParseFloat(string value)
    {
        try
        {
            return float.Parse(value.Replace(",", "."));

        }catch (Exception exception)
        {
            Debug.LogError($"[BlockBuilderUIController] Error parsing float: {exception.Message}");
            return 0f;
        }
    }
}
