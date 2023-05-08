using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeViewController : MonoBehaviour
{
    private void Start()
    {
        EventManager.TriggerCameraViewChange(GetComponent<Camera>());

        EventManager.OnResetSelecteObject += OnResetSelecteObject;
    }

    private void OnResetSelecteObject()
    {
        EventManager.TriggerObjectSelected(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.TriggerObjectSelected(transform);
        }
    }

    private void OnEnable()
    {
        EventManager.TriggerObjectSelected(transform);
    }
}
