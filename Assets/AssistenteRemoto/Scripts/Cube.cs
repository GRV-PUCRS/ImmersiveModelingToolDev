using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [System.Obsolete]
    void Update()
    {
        transform.RotateAround(Vector3.forward, Time.deltaTime * 5);
    }
}
