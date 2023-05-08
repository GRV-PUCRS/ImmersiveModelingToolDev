using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//code from: https://forum.unity.com/threads/how-to-oculus-quest-hand-tracking-pointerpose-pinch.1170538/
[RequireComponent(typeof(OVRHand))]
public class HandPointer : MonoBehaviour
{
    [SerializeField] private OVRInputModule ovrInputModule;
    [SerializeField] private OVRRaycaster ovrRaycaster;

    [SerializeField] private Transform ovrHandTransform;

    void Start()
    {

        ovrInputModule.rayTransform = ovrHandTransform;
        ovrRaycaster.pointer = ovrHandTransform.gameObject;

    }

    void Update()
    {
        ovrInputModule.rayTransform = ovrHandTransform;
        ovrRaycaster.pointer = ovrHandTransform.gameObject;
    }
}
