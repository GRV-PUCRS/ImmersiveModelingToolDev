using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardTypeProvider : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] public TextMeshProUGUI _txtTextPreview;


    public void SetActive(bool value)
    {
        _view.SetActive(value);
    }
}
