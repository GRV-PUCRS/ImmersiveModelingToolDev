using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Temp : MonoBehaviour
{
    public Renderer container;
    public Transform objs;

    [Range(1, 6)] public int rows = 3;
    [Range(1, 6)] public int cols = 3;

    [Range(0.1f, 1)] public float maxSizeScale = 1f;

    private float offsetY;
    private float offsetX;

    public void Start()
    {
        offsetX = container.bounds.size.x / cols;
        offsetY = container.bounds.size.y / rows;

        Vector3 initPos = new Vector3(container.transform.position.x + container.transform.right.x * container.bounds.extents.x - offsetX / 2,
                                      container.transform.position.y + container.transform.up.y * container.bounds.extents.y - offsetY / 2,
                                      container.transform.position.z);

        int counter = 0;

        Vector3 maxSize = new Vector3(offsetX, offsetY, container.bounds.size.z) * maxSizeScale;


        foreach (DragUI dragUI in objs.GetComponentsInChildren<DragUI>())
        {
            Renderer r = dragUI.GetComponent<Renderer>();

            if (r == null)
            {
                Debug.LogError("Dragui nao contem Renderer");
                continue;
            }

            r.transform.parent.position = initPos - new Vector3(offsetX * (counter / rows), offsetY * (counter % cols), 0);
            counter++;

            Vector3 oldScale = r.transform.parent.transform.localScale;
            r.transform.parent.transform.localScale = new Vector3(oldScale.x * maxSize.x / r.bounds.size.x, oldScale.y * maxSize.y / r.bounds.size.y, oldScale.z * maxSize.z / r.bounds.size.z);
        }
    }
}
