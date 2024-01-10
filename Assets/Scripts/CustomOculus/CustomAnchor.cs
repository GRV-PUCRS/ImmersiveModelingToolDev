// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using System;
using UnityEngine;

/// <summary>
/// Specific functionality for spawned anchors
/// </summary>
[RequireComponent(typeof(OVRSpatialAnchor))]
public class CustomAnchor : MonoBehaviour
{
    public const string NumUuidsPlayerPref = "numUuids";
    private OVRSpatialAnchor _spatialAnchor;

    private void Awake()
    {
        _spatialAnchor = GetComponent<OVRSpatialAnchor>();
    }

    public void Save(Action<string> anchorID = null, Action saveFailed = null)
    {
        if (!_spatialAnchor) return;

        _spatialAnchor.Save((anchor, success) =>
        {   
            if (!success)
            {
                Log($"[Save] save failed on {anchor.Uuid}");
                saveFailed?.Invoke();
                return;
            }

            /*
            // Write uuid of saved anchor to file
            if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
            {
                PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
            }

            int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
            PlayerPrefs.SetString("uuid" + playerNumUuids, anchor.Uuid.ToString());
            PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
            */
            Log($"[Save] save completed {anchor.Uuid}");
            anchorID?.Invoke(anchor.Uuid.ToString());
        });
    }

    public void Erase()
    {
        if (!_spatialAnchor) return;

        Log($"[Erase] Erasing {_spatialAnchor.Uuid} ...");
        _spatialAnchor.Erase((anchor, success) =>
        {
            Log($"[Erase] Erase {_spatialAnchor.Uuid} completed");
        });
    }

    private static void Log(string message) => Debug.Log($"[CustomAnchor]{message}");
}
