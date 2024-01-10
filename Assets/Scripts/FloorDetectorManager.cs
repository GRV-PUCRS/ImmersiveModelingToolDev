using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static OVRSceneManager;

public class FloorDetectorManager : MonoBehaviour
{
    [SerializeField] private  OVRSceneManager sceneManager;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform vrEnvironment;

    private void Awake()
    {
#if UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
        sceneManager.NoSceneModelToLoad += OnNoSceneModelLoaded;
        sceneManager.UnexpectedErrorWithSceneCapture += OnUnexpectedErrorWithSceneCapture;

        EventManager.OnVRModeActive += OnVRModeActive;
    }

    void OnSceneLoaded()
    {
        Debug.Log("[SceneLoader] On Scene Loaded");

        var anchors = new List<OVRSceneAnchor>();
        OVRSceneAnchor.GetSceneAnchors(anchors);
        foreach (var sceneAnchor in anchors)
        {
            OVRSemanticClassification classification = sceneAnchor.GetComponentInChildren<OVRSemanticClassification>();

            if (classification == null) continue;
            if (classification.Labels.Count == 0 || classification.Labels[0] != Classification.Floor) continue;

            floor.position = new Vector3(floor.position.x, sceneAnchor.transform.position.y, floor.position.z);
            vrEnvironment.position = new Vector3(vrEnvironment.position.x, sceneAnchor.transform.position.y, vrEnvironment.position.z);

            Debug.Log("[SceneLoader] Floor Anchor detected");

            return;
        }

        Debug.Log("[SceneLoader] Floor Anchor not detected");
    }

    void OnNoSceneModelLoaded()
    {
        Debug.Log("[SceneLoader] No Scene Loaded");
        floor.gameObject.SetActive(false);
    }

    void OnUnexpectedErrorWithSceneCapture()
    {
        Debug.Log("[SceneLoader] Unexpected Error With Scene Capture");
    }

    private void OnVRModeActive(bool newValue)
    {
        floor.gameObject.SetActive(!newValue);
    }
}
