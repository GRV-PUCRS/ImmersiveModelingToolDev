using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplicateObjectActionUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private BoxCollider _canvasCollider;
    [SerializeField] private RectTransform _stepParent;
    [SerializeField] private GameObject _stepPrefab;

    [SerializeField] private int _defaultHeight = 200;
    [SerializeField] private int _heightPerLine = 100;
    [SerializeField] private int _StepPerLine = 10;

    [SerializeField] private ReplicateObjectController _controller;


    public void UpdateUI(int steps, int currentStep)
    {
        _stepParent.DestroyChildren();

        int heightToAdd = 0;

        for (int i = 0; i < steps; i++)
        {
            heightToAdd = i % _StepPerLine == 0 ? heightToAdd + _heightPerLine : heightToAdd;

            var instance = Instantiate(_stepPrefab, _stepParent).GetComponent<StepElementTemplateController>();
            instance.Setup(i == currentStep, i+1);
        }

        var sizeDelta = _canvasRectTransform.sizeDelta.SetY(_defaultHeight + heightToAdd);
        _canvasRectTransform.sizeDelta = sizeDelta;
        _canvasCollider.size = sizeDelta;
    }

    public void Complete()
    {
        var stepSelected = new List<int>();

        int i = 0;
        foreach (RectTransform child in _stepParent)
        {
            if (child.TryGetComponent<Toggle>(out var toggle))
            {
                if (toggle.isOn) stepSelected.Add(i);
            }

            i++;
        }

        _controller.Confirm(stepSelected);
    }

    public void Cancel()
    {
        _controller.Cancel();
    }
}
