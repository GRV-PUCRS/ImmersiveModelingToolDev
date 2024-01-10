using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSizeHandler : MonoBehaviour
{
    [SerializeField] private Vector3 _desirableSize;

    public Vector3 GetDesirableSize() => _desirableSize;
}
