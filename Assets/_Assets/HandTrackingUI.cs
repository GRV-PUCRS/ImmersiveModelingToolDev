using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandTrackingUI : MonoBehaviour
{
    public Transform hand;
    public OVRInputModule inputModule;

    private void Start()
    {
        inputModule.rayTransform = hand;
    }
}
