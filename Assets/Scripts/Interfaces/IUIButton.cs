using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIButton
{
    void OnSelected(ObjectSelector controller);

    void OnUnselected(ObjectSelector controller);

    void OnSubmited(ObjectSelector controller);

    void OnReleased(ObjectSelector controller);
}
