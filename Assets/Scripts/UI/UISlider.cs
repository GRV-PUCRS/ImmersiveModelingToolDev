using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlider : MonoBehaviour, IUIButton
{
    [SerializeField] private RectTransform handler;
    [SerializeField] private RectTransform beginPoint;
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private RectTransform fill;

    [SerializeField, Range(0f, 1f)] private float sliderValue;
    [SerializeField, Min(0.001f)] private float maxValue = 1;
    [SerializeField, Min(0.001f)] private float minValue = 0;

    private ObjectSelector controller;

    public UnityEvent<float> OnValueChange; 

    // Update is called once per frame
    void Update()
    {
        if (controller == null) return;

        float dist = Vector3.Distance(handler.position, controller.transform.position);

        Vector3 projectedPoint = controller.transform.position + controller.transform.forward * dist;

        if (Vector3.Distance(projectedPoint, handler.position) < 0.2f)
        {
            sliderValue = Mathf.Clamp(Vector3.Distance(projectedPoint, beginPoint.position) / Vector3.Distance(beginPoint.position, endPoint.position), 0f, 1f);

            UpdateUI();
            OnValueChange?.Invoke(sliderValue);
        }
    }

    public void OnSelected(ObjectSelector controller)
    {
    }

    public void OnUnselected(ObjectSelector controller)
    {

    }

    public void OnSubmited(ObjectSelector controller)
    {
        this.controller = controller;

        if (controller.IsLeft)
            InputController.Instance.OnLeftIndexTriggerUp.AddListener(Deselect);
        else
            InputController.Instance.OnRightIndexTriggerUp.AddListener(Deselect);
    }

    public void OnReleased(ObjectSelector controller)
    {

    }

    public void Deselect()
    {
        if (controller.IsLeft)
            InputController.Instance.OnLeftIndexTriggerUp.RemoveListener(Deselect);
        else
            InputController.Instance.OnRightIndexTriggerUp.RemoveListener(Deselect);

        controller = null;
    }

    private void UpdateUI()
    {
        handler.anchorMin = new Vector2(sliderValue, handler.anchorMin.y);
        handler.anchorMax = new Vector2(sliderValue, handler.anchorMax.y);
        fill.anchorMax = new Vector2(sliderValue, fill.anchorMax.y);
    }

    private void OnValidate()
    {
        UpdateUI();
        OnValueChange?.Invoke(sliderValue);
    }

    public void ChangeValue(float value, bool notify)
    {
        sliderValue = Mathf.Clamp(value / (MaxValue - MinValue), 0f, 1f);
        UpdateUI();

        if (notify)
            OnValueChange?.Invoke(sliderValue);
    }

    public float MaxValue { get => maxValue; set => maxValue = value; }
    public float MinValue { get => minValue; set => minValue = value; }
    public float Value { get => sliderValue * (MaxValue - MinValue); set => ChangeValue(value, true); }
}
