using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{
    void BeginDrag(Transform pivot);
    void EndDrag();
}
