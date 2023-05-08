using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraController : MonoBehaviour
{
    [SerializeField] private Transform tagTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform arCameraTransform;

    public void OnARCameraReset(object[] message)
    {
        Vector3 arCanera = tagTransform.position + (Vector3)message[0];

        arCameraTransform.localPosition = cameraTransform.position - arCanera;
    }
}
