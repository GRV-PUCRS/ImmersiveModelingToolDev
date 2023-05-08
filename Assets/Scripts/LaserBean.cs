using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBean : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private LayerMask target;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit info, 100, target))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, info.point);
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward*5);
        }
    }
}
