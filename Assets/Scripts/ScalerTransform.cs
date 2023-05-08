using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerTransform : MonoBehaviour
{
    [Header("Axis to tranform")]
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;


    public void Transform(float transformModifier)
    {
        float newX = (x) ? transformModifier : transform.localScale.x;
        float newY = (y) ? transformModifier : transform.localScale.y;
        float newZ = (z) ? transformModifier : transform.localScale.z;

        transform.localScale = new Vector3(newX, newY, newZ);
    }
}
