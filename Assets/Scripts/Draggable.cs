using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Draggable : MonoBehaviour, IDraggable
{
    public UnityEvent onDragBegin;
    public UnityEvent onDragEnd;

    public void BeginDrag(Transform pivot)
    {
        onDragBegin?.Invoke();
    }

    public void EndDrag()
    {
        onDragEnd?.Invoke(); 
    }
}
