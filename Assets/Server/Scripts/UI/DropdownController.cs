using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class DropdownController : MonoBehaviour
{
    [SerializeField] private string prefabsFolder;
    [SerializeField] private TMP_Dropdown dropdown;

    public UnityEventString onPrefabChoose;

    private void Start()
    {
        var prefabs = Directory.GetFiles(Application.dataPath + "/Resources/" + prefabsFolder).Where(name => !name.EndsWith(".meta"));

        dropdown.onValueChanged.AddListener(OnChangeDropdown);
        dropdown.options.Clear();

        foreach (string prefab in prefabs)
        {
            Debug.Log(prefab);
            string[] tokens = prefab.Replace('\\', '/').Split('/');
            string[] filesTokens = tokens[tokens.Length - 1].Split('.');
            if (!ExtensionAllowed.IsAllowedExtension(filesTokens[1])) continue;


            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = tokens[tokens.Length - 1] });
        }

        OnChangeDropdown(0);
    }

    public void OnChangeDropdown(int choice)
    {
        if (choice < dropdown.options.Count && choice < dropdown.options.Count) 
        {
            onPrefabChoose?.Invoke(dropdown.options[choice].text);
        }
    }
}
