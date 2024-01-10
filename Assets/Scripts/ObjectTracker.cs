using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectTracker : MonoBehaviour
{
    [SerializeField] private GameObject objectIdView;
    [SerializeField] private GameObject objectMenuView;
    [SerializeField] private TextMeshProUGUI txtObjectID;
    [SerializeField] private TextMeshProUGUI txtRoom;

    private Transform objectTransform;

    private void Awake()
    {
        objectTransform = transform;
        IsRegistred = false;
    }

    private void Start()
    {
        if (ObjectTrackerManager.Instance == null || !NetworkPhoton.Instance.InRoom)
        {
            Debug.LogWarning($"[{this}][StarTrackingObject] ObjectTrackerManager instance == NULL", this);
            enabled = false;
        }

        txtRoom.text = $"Room: {NetworkPhoton.Instance.RoomID}";

        objectIdView.SetActive(false);
        objectMenuView.SetActive(true);
    }

    public void StarTrackingObject()
    {
        string result = ObjectTrackerManager.Instance.AddObject(this);

        if (result.Length == 0) return;

        ObjectID = result;

        IsRegistred = true;
        txtObjectID.text = ObjectID;

        objectIdView.SetActive(true);
        objectMenuView.SetActive(false);
    }

    private void OnDisable()
    {
        if (!IsRegistred) return;

        ObjectTrackerManager.Instance.RemoveObject(this);
        IsRegistred = false;
    }

    public void UpdateTransform(Vector3 position, Vector3 eulerAngle, Vector3 scale)
    {
        objectTransform.localPosition = position;
        objectTransform.localEulerAngles = eulerAngle;
        objectTransform.localScale = scale;
    }

    public string ObjectID { get; private set; }
    public bool IsRegistred { get; private set; }
}
