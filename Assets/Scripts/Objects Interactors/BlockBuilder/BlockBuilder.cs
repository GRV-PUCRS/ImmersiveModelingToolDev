using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuilder : MonoBehaviour
{
    [SerializeField] private BlockBuilderUIController _builderUIController;

    private Transform _instance;

    public Vector3 BlockScale { get => _instance.localScale; }


    public void ChangeAxis(Vector3 axisValues)
    {
        _instance.localScale = axisValues;
    }

    public void Complete()
    {
        ClearAction();
    }

    private void ClearAction()
    {
        EventManager.OnExitSession -= EventManager_OnExitSession;
        EventManager.OnStageChange -= EventManager_OnStageChange;
        Destroy(gameObject);
    }

    private void EventManager_OnStageChange()
    {
        ClearAction();
    }

    private void EventManager_OnExitSession()
    {
        ClearAction();
    }

    public void SetObject(Transform transform)
    {
        _instance = transform;
        _builderUIController.InitValues(transform.localScale);
    }

    private void OnEnable()
    {
        EventManager.OnExitSession += EventManager_OnExitSession;
        EventManager.OnStageChange += EventManager_OnStageChange;
    }
}
