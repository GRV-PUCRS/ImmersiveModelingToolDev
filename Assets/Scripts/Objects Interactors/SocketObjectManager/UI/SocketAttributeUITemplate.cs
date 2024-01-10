using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SocketAttributeUITemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtMethodName;
    [SerializeField] private TextMeshProUGUI txtArgs;
    [SerializeField] private Toggle toggleTrigger;

    public void SetInfo(SocketObjectUIController controller, string methodName, List<string> args, bool isActive)
    {
        txtMethodName.text = methodName;
        
        txtArgs.text = "";
        args.ForEach(arg => txtArgs.text += $"{arg};");

        toggleTrigger.isOn = isActive;

        toggleTrigger.onValueChanged.AddListener((value) => controller.SetAttributeActive(methodName, value));
    }

    public void ToggleActive()
    {
        toggleTrigger.isOn = !toggleTrigger.isOn;
    }
}
