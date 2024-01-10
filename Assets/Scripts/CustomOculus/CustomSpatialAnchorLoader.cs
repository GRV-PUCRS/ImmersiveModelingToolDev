using System;
using UnityEngine;

public class CustomSpatialAnchorLoader : MonoBehaviour
{
    /*
    public void LoadAnchorsByUuid()
    {
        // Get number of saved anchor uuids
        if (!PlayerPrefs.HasKey(Anchor.NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(Anchor.NumUuidsPlayerPref, 0);
        }

        var playerUuidCount = PlayerPrefs.GetInt("numUuids");
        Log($"[LoadAnchorsByUuid] Attempting to load {playerUuidCount} saved anchors.");
        if (playerUuidCount == 0)
            return;

        var uuids = new Guid[playerUuidCount];
        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            var currentUuid = PlayerPrefs.GetString(uuidKey);
            Log("[LoadAnchorsByUuid] QueryAnchorByUuid: " + currentUuid);

            uuids[i] = new Guid(currentUuid);
        }

        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }
    */

    private void Load(OVRSpatialAnchor.LoadOptions options, Action<Vector3, Quaternion> OnAnchorLoaded, Action OnLoadFailed) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null)
        {
            Log("[Load] Query failed.");
            OnLoadFailed?.Invoke();

            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                OnAnchorLoaded?.Invoke(anchor.Pose.position, anchor.Pose.rotation);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize((localizedAnchor, result) => 
                {
                    if (result)
                    {
                        OnAnchorLoaded?.Invoke(localizedAnchor.Pose.position, localizedAnchor.Pose.rotation);
                    }
                    else
                    {
                        OnLoadFailed?.Invoke();
                    }
                });
            }
        }
    });

    public void LoadAnchorAsync(string anchorID, Action<Vector3, Quaternion> OnAnchorLoaded, Action OnLoadFailed)
    {
        Log("[LoadAnchorAsync] QueryAnchorByUuid: " + anchorID);

        try
        {
            var uuid = new Guid[1] { new Guid(anchorID) };

            Load(new OVRSpatialAnchor.LoadOptions
            {
                Timeout = 0,
                StorageLocation = OVRSpace.StorageLocation.Local,
                Uuids = uuid
            }, OnAnchorLoaded, OnLoadFailed);
        
        }catch (Exception error)
        {
            OnLoadFailed?.Invoke();
            Log($"[LoadAnchorAsync] Erro ao tentar carregar ancora com id {anchorID}: {error.Message}");
        }
    }

    private static void Log(string message) => Debug.Log($"[OCULUS][CustomSpatialAnchorsLoader]{message}");
}
