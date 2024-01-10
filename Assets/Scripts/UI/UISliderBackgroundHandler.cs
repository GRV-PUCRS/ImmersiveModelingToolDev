using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UISliderBackgroundHandler : MonoBehaviour, IUIButton
{
    [SerializeField] private UISlider _uiSlider;
    [SerializeField] private Transform _beginPoint;
    [SerializeField] private Transform _endPoint;


    public void OnSelected(ObjectSelector controller) { }

    public void OnUnselected(ObjectSelector controller) {}

    public void OnSubmited(ObjectSelector controller) 
    {
        float dist = Vector3.Distance(transform.position, controller.transform.position);
        Vector3 projectedPoint = controller.transform.position + controller.transform.forward * dist;

        float maxDist = Vector3.Distance(_beginPoint.position, _endPoint.position);
        float pointDist = Vector3.Distance(_beginPoint.position, projectedPoint);

        _uiSlider.Value = (pointDist / maxDist) * (_uiSlider.MaxValue - _uiSlider.MinValue);
    }

    public void OnReleased(ObjectSelector controller) {}
}
