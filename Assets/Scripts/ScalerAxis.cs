using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerAxis : MonoBehaviour
{
    [SerializeField] private ScalerTransform[] transforms;
    [SerializeField] private Transform ghost;

    private Vector3 oldPosition;
    private Vector3 anchorPosition;
    private float anchorDistance;
    private Transform controller;
    private Transform oldParent;

    private void Update()
    {
        if (controller == null) return;

        float newDistance = Vector3.Distance(transform.position, anchorPosition);
        float distanceMagnitude = newDistance / Mathf.Max(0.001f, anchorDistance);

        transform.localPosition = new Vector3(ghost.position.x * -1, 0, 0);

        foreach (ScalerTransform st in transforms)
        {
            st.Transform(ghost.position.x * -1);
        }
    }

    public void DragBegin()
    {
        oldPosition = transform.position;
        controller = InputController.Instance.RightController;
        anchorPosition = controller.position;
        anchorDistance = Vector3.Distance(anchorPosition, ghost.position);

        oldParent = ghost.parent;
        ghost.parent = controller;
    }

    public void DragEnd()
    {
        ghost.parent = oldParent;
        ghost.localPosition = Vector3.zero;
    }
}
