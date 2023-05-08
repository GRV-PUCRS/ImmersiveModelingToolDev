using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlider : MonoBehaviour, IDraggable
{
    [SerializeField] private RectTransform handler;
    [SerializeField] private RectTransform handlerGhost;
    [SerializeField] private int beginPosition;
    [SerializeField] private int endPosition;
    [SerializeField] private float uiScale;
    [SerializeField] private Slider sliderController;

    private bool isDragging = false;

    // Update is called once per frame
    void Update()
    {
        if (!isDragging) return;

        
    }

    public void BeginDrag(Transform pivot)
    {
        isDragging = true;
        Debug.Log("Begin Drag");
        /*
        handlerGhost.localPosition = handler.localPosition;
        handlerGhost.parent = InputController.Instance.RightController;
        */

        ExecuteEvents.Execute(sliderController.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.beginDragHandler);
    }

    public void EndDrag()
    {
        isDragging = false;
        Debug.Log("End Drag");
        //handlerGhost.parent = handler.parent;
        ExecuteEvents.Execute(sliderController.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.endDragHandler);
    }

    public void Teste()
    {
        Debug.Log("-----> " + name);
    }
}
