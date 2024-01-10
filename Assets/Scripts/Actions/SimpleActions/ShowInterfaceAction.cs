using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInterfaceAction : AbstractSimpleAction
{
    [SerializeField] private GameObject _showInterfaceAction;
    private GameObject _instance = null;

    private void DestroyInterface()
    {
        if (_instance == null) return;

        Destroy(_instance);
    }

    public override void ApplyAction(List<GameObject> sceneElements)
    {
        if (_instance != null) DestroyInterface();

        ObjectStore.Instance.RetrieveObjectToStore(transform);

        _instance = Instantiate(_showInterfaceAction);

        if (_instance.TryGetComponent<IShowInterface>(out var showInterfaceActionInstance))
        {
            showInterfaceActionInstance.ApplyAction(sceneElements);

            _instance.transform.position = InputController.Instance.HMD.position + InputController.Instance.HMD.forward * 0.75f;
            _instance.transform.rotation = InputController.Instance.HMD.rotation;
            _instance.transform.Rotate(Vector3.up, 180f);
        }
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnExitSession += DestroyInterface;
        EventManager.OnStageChange += DestroyInterface;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.OnExitSession -= DestroyInterface;
        EventManager.OnStageChange -= DestroyInterface;
    }
}
