using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public TextMeshProUGUI _inputField;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            _inputField.text = $"{"30.00".Replace(".", ",")}  {float.Parse("30.00".Replace(".", ","))}";
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _inputField.text = $"{float.Parse("30,00", CultureInfo.InvariantCulture)}";
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            _inputField.text = $"{float.Parse("30.00", CultureInfo.InvariantCulture)}";
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            _inputField.text = $"{float.Parse("30.00", CultureInfo.InvariantCulture)}";
        }
    }

}
