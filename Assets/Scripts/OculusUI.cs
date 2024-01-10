using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OculusUI : Singleton<OculusUI>
{
    [SerializeField] private GameObject view;

    public void ResetVR(bool isActive)
    {
        view.SetActive(isActive);
    }
}
