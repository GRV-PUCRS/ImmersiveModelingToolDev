using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayController : MonoBehaviour
{
    [SerializeField] private LayerMask target;
    [SerializeField] private float lineDistance = 5f;

    private LineRenderer lineRenderer;
    private bool updateRayRequired = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit info, target))
        {
            lineRenderer.enabled = true;

            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(info.point, lineRenderer.transform.position));

            updateRayRequired = true;
        }
        else
        {
            if (updateRayRequired)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.forward * lineDistance);

                updateRayRequired = false;
            }
        }
    }
}
