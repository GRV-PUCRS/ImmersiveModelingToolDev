using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectTrackerAppManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputRoom;
    [SerializeField] private TMP_InputField inputObject;
    [SerializeField] private TextMeshProUGUI txtStatus;

    [SerializeField] private float step = 0.1f;

    private Vector3 position = Vector3.zero;
    private object[] message = new object[5];

    private void Awake()
    {
        message[0] = ObjectTrackerManager.ObjectTrackerMessageType.UPDATE_OBJECT;
        message[1] = null;
        message[2] = null;
        message[3] = Vector3.zero;
        message[4] = Vector3.one;

    }

    public void Connect()
    {
        txtStatus.text = "Conectando ...";
        NetworkPhoton.Instance.JoinRoom(inputRoom.text);
    }

    public void ChangeXValue(bool toIncrease)
    {
    }


    private void OnJoinedRoom(int players)
    {
        txtStatus.text = $"Online ({players})";
    }

    private void OnJoinedRoomFailed(string error)
    {
        txtStatus.text = $"Error: {error}";
    }

    private void Start()
    {
        NetworkPhoton.Instance.OnJoinedRoomEvent.AddListener(OnJoinedRoom);
        NetworkPhoton.Instance.OnJoinRoomFailedEvent.AddListener(OnJoinedRoomFailed);
    }

}
