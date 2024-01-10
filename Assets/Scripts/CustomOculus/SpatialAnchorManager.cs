using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CustomSpatialAnchorLoader))]
public class SpatialAnchorManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _anchorPrefab;

    private CustomSpatialAnchorLoader _loader;
    private CustomAnchor _currentAnchor;

    public event Action<string, Vector3, Vector3> OnAnchorLoaded;
    public event Action OnLoadFailed;

    private void Start()
    {
        _currentAnchor = null;

        _loader = GetComponent<CustomSpatialAnchorLoader>();
    }

    private void Loader_OnLoadAnchor(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool result)
    {
        if (!result)
        {
            Log($"[Loader_onLoadAnchor] {unboundAnchor.Uuid} not loaded");
            return;
        }

        Log($"[Loader_onLoadAnchor] {unboundAnchor.Uuid} loaded");

        var pose = unboundAnchor.Pose;
        var instance = Instantiate(_anchorPrefab, pose.position, pose.rotation);
        instance.transform.SetParent(transform);

        if (instance.TryGetComponent<OVRSpatialAnchor>(out var anchor))
        {
            unboundAnchor.BindTo(anchor);
            Log($"[Loader_onLoadAnchor] {unboundAnchor.Uuid} bound");
            OnAnchorLoaded?.Invoke(anchor.Uuid.ToString(), pose.position, pose.rotation.eulerAngles);
        }
        else
        {
            Log($"[Loader_onLoadAnchor] {unboundAnchor.Uuid} not bound");
            OnLoadFailed?.Invoke();
        }
    }

    public Transform CreateAnchor(Vector3 position, Quaternion rotation)
    {
        if (_currentAnchor != null)
        {
            EraseAnchor();
        }

        Log($"[CreateAnchor] Create Anchor");
        var instance = Instantiate(_anchorPrefab, position, rotation).transform;

        _currentAnchor = instance.GetComponentInChildren<CustomAnchor>();

        return instance;
    }

    public void SaveAnchor(Action<string> onSaveCompleded)
    {
        if (_currentAnchor == null) return;

        Log($"[SaveAnchor] Saving anchor");

        _currentAnchor.Save(id => onSaveCompleded?.Invoke(id));
    }

    public void EraseAnchor()
    {
        if (_currentAnchor == null) return;

        Log($"[EraseAnchor] Erasing anchors");

        _currentAnchor.Erase();
        Destroy(_currentAnchor.gameObject);
    }

    public void DestroyAnchor()
    {
        if (_currentAnchor == null) return;
        Destroy(_currentAnchor.gameObject);
    }

    public void LoadAnchor(string anchorId, Action<Vector3, Quaternion> OnAnchorLoaded, Action OnLoadFailed)
    {
        Log($"[LoadLastAnchor] Loading anchor {anchorId}");
        _loader.LoadAnchorAsync(anchorId, OnAnchorLoaded, OnLoadFailed);
    }

    private static void Log(string message) => Debug.Log($"[SpatialAnchorTeste]{message}");
}
