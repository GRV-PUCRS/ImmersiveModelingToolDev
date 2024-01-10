using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(BoxCollider))]
public class WorldCanvasInteractor : DragUI
{
    [SerializeField] private Transform _backgroundMeshTransform;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _rectTransformReference;

    private bool initialized = false;

    protected override void Awake()
    {
        base.Awake();

        if (!initialized)
        {
            UpdateSize();
        }
    }

    protected override void HandleScale()
    {
        float newDist = Vector3.Distance(pivot1.position, pivot2.position);
        float scaleFactor = Mathf.Min(Mathf.Abs(newDist - oldDist) / oldDist, maxScaleTransformation);

        if (scaleFactor > 0.1f)
        {
            scaleFactor = (newDist > oldDist) ? scaleFactor : -scaleFactor;

            Vector2 canvasValues = new Vector2();
            canvasValues.x = OculusManager.Instance.ScaleX ? _rectTransform.sizeDelta.x * scaleFactor : 0;
            canvasValues.y = OculusManager.Instance.ScaleY ? _rectTransform.sizeDelta.y * scaleFactor : 0;

            Vector2 newScale = Vector2.zero;
            newScale.x = _rectTransform.sizeDelta.x + canvasValues.x;
            newScale.y = _rectTransform.sizeDelta.y + canvasValues.y;


            _rectTransform.sizeDelta = newScale;
            UpdateSize();

            oldDist = newDist;

            SoundManager.Instance.PlaySound(scaleFactor < 0 ? SoundManager.Instance.decreaseScale : SoundManager.Instance.increaseScale);
        }
    }

    private void UpdateSize()
    {
        Vector3 newScale = new Vector3(_rectTransformReference.rect.width, _rectTransformReference.rect.height) * 0.001f;

        Vector3 backgroundValues = newScale;
        backgroundValues.z = _backgroundMeshTransform.localScale.z;
        _backgroundMeshTransform.localScale = backgroundValues;

        _collider.size = newScale;
    }

    public void SetNewSize(Vector3 scale)
    {
        _rectTransform.sizeDelta = scale;
        UpdateSize();
        initialized = true;
    }

    public void SetRectTransformReference(RectTransform reference)
    {
        _rectTransformReference = reference;
        UpdateSize();
    }
}
