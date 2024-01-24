using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ColorPickerUI : Singleton<ColorPickerUI>
{
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject errorScreen;

    [SerializeField] private TextMeshProUGUI txtMeshRenderer;
    [SerializeField] private UISlider sliderRed;
    [SerializeField] private UISlider sliderGreen;
    [SerializeField] private UISlider sliderBlue;

    [SerializeField] private Image newColor;

    private List<MeshRenderer> meshRenderers;
    private int index = 0;
    private ChangeColorAction colorAction;
    private List<GameObject> _groups;

    private void Start()
    {
        ChangeRGBValue(0);
    }

    public void ChangeRGBValue(float value)
    {
        UpdateColor(newColor, GetColor());
    }

    public void Back()
    {
        colorAction.UnselectComponent(index);

        index = (meshRenderers.Count + index - 1) % meshRenderers.Count;

        colorAction.SelectComponent(index);

        UpdateColorInfo();
    }

    public void Next()
    {
        colorAction.UnselectComponent(index);

        index = (index + 1) % meshRenderers.Count;

        colorAction.SelectComponent(index);

        UpdateColorInfo();
    }

    public void UpdateColorInfo()
    {
        txtMeshRenderer.text = meshRenderers[index].name;
    }

    public void AddColor()
    {
        colorAction.AddColor(index, GetColor());
        UpdateColorInfo();
    }

    public void RmvColor()
    {
        colorAction.RmvColor(index);
        UpdateColorInfo();
    }

    public void UpdateColor(Image image, Color color)
    {
        image.color = color;
    }

    public void SetInfo(List<MeshRenderer> renderers)
    {
        if (renderers.Count == 0)
        {
            errorScreen.SetActive(true);
        }
        else
        {
            errorScreen.SetActive(false);
            view.SetActive(true);

            index = 0;
            meshRenderers = renderers;

            UpdateColorInfo();
            UpdateColor(newColor, GetColor());
            colorAction.SelectComponent(index);
        }
    }

    public void SetGroup(List<GameObject> sceneElements)
    {
        _groups = sceneElements;
    }

    private Color GetColor()
    {
        return new Color(sliderRed.Value / 255f, sliderGreen.Value / 255f, sliderBlue.Value / 255f);
    }

    public void Complete()
    {
        view.SetActive(false);
        colorAction.Completed();
    }

    public void SetColorAction(ChangeColorAction colorAction)
    {
        this.colorAction = colorAction;
    }
}
