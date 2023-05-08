using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialScreenController : MonoBehaviour
{
    private void Update()
    {
        transform.position = InputController.Instance.HMD.position + InputController.Instance.HMD.forward * 1.5f;
        transform.rotation = Quaternion.Euler(InputController.Instance.HMD.eulerAngles);

        this.enabled = false;
    }
}
