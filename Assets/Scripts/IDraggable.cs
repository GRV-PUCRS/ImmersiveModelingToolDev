using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{
    void BeginDrag(ObjectSelector controller);
    void EndDrag();
}
