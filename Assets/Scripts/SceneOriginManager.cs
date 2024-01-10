using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOriginManager : MonoBehaviour
{
    [SerializeField] private Transform _rightAxisTransform;
    [SerializeField] private Transform _leftAxisTransform;
    [SerializeField] private GameObject _sceneOriginParent;
    [SerializeField] private SpatialAnchorManager _spatialAnchorManager;
    [SerializeField] private OculusManager _oculusManager;
    [SerializeField] private GameObject _sessionEnvironment;
    [SerializeField] private GameObject _tutorialInstance;

    private bool _isSettingOrigin;

    private void Awake()
    {
        _isSettingOrigin = false;

        EventManager.OnEnterSession += EventManager_OnEnterSession;
        EventManager.OnExitSession += EventManager_OnExitSession;
    }

    private void EventManager_OnEnterSession()
    {
        InputController.Instance.OnStartButtonPressed.AddListener(OnStartButtonPressed);
        InputController.Instance.OnOneButtonPressed.AddListener(OnOneButtonPressed);

        _isSettingOrigin = false;
    }

    private void EventManager_OnExitSession()
    {
        InputController.Instance.OnStartButtonPressed.RemoveListener(OnStartButtonPressed);
        InputController.Instance.OnOneButtonPressed.RemoveListener(OnOneButtonPressed);

        _isSettingOrigin = false;
    }

    private void UpdateMode()
    {
        _tutorialInstance.SetActive(_isSettingOrigin);
        _rightAxisTransform.gameObject.SetActive(_isSettingOrigin);
        _leftAxisTransform.gameObject.SetActive(_isSettingOrigin);

        if (_isSettingOrigin)
        {
            Log($"[OnStartButtonPressed] Enter in Set Origin Mode");
            InputController.Instance.OnRightIndexTriggerDown.AddListener(OnRightIndexTriggerDown);
            _sessionEnvironment.SetActive(false);
        }
        else
        {
            Log($"[OnStartButtonPressed] Exit Set Origin Mode");
            InputController.Instance.OnRightIndexTriggerDown.RemoveListener(OnRightIndexTriggerDown);

            var plan = _oculusManager.TaskManager.GetPlan();
            _spatialAnchorManager.SaveAnchor(newID =>
            {
                plan.SpatialAnchorOriginID = newID;

                Log($"[OnStartButtonPressed] Save new Anchor {newID} for plan {plan.Name}");
                _spatialAnchorManager.DestroyAnchor();
            });

            _sessionEnvironment.SetActive(true);
        }
    }

    private void OnStartButtonPressed()
    {
        if (!_oculusManager.TaskManager.gameObject.activeInHierarchy) return;

        _isSettingOrigin = !_isSettingOrigin;

        UpdateMode();
    }

    public void SetSettingOriginMode(bool isOn)
    {
        _isSettingOrigin = isOn;

        UpdateMode();
    }

    private void OnRightIndexTriggerDown()
    {
        _spatialAnchorManager.CreateAnchor(_rightAxisTransform.position, _rightAxisTransform.rotation);
        _oculusManager.SetEnvironmentPosition(_rightAxisTransform.position, _rightAxisTransform.rotation);

        //SetSettingOriginMode(false);

        Log($"[OnRightIndexTriggerDown] Set new Origin position");
    }

    private void OnOneButtonPressed()
    {
        _sceneOriginParent.SetActive(!_sceneOriginParent.activeInHierarchy);
    }

    private static void Log(string message) => Debug.Log($"[SceneOriginManager]{message}");
}
