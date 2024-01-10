using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController
{
    private List<MeshRenderer> renderers;
    private List<Color> colors;

    public ColorController(GameObject obj)
    {
        colors = new List<Color>();
        renderers = new List<MeshRenderer>();

        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            if (!renderer.material.HasProperty("_Color")) continue;

            renderers.Add(renderer);
            colors.Add(renderer.material.color);
        }
    }

    public void AddColor(int index, Color color)
    {
        if (index < 0 || index >= renderers.Count) return;

        renderers[index].material.color = color;
    }

    public void RmvColor(int index)
    {
        if (index < 0 || index >= renderers.Count) return;

        renderers[index].material.color = colors[index];
    }

    public void SelectComponent(int index)
    {
        if (index < 0 || index >= renderers.Count) return;

        MeshRenderer renderer = renderers[index];

        var outline = renderer.gameObject.GetOrAddComponent<Outline>();
        outline.EnableOutline();
    }

    public void UnselectComponent(int index)
    {
        if (index < 0 || index >= renderers.Count) return;

        MeshRenderer renderer = renderers[index];

        var outline = renderer.gameObject.GetOrAddComponent<Outline>();
        outline.DisableOutline();
    }

    public void UpdateColors()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material.color = colors[i];
        }
    }

    public void UpdateColors(List<Vector3> savedColors)
    {
        if (savedColors.Count != MeshRenderers.Count) return;

        for (int i = 0; i < MeshRenderers.Count; i++)
        {
            Color color = new Color(savedColors[i].x, savedColors[i].y, savedColors[i].z);

            renderers[i].material.color = color;
            colors[i] = color;
        }
    }

    public List<MeshRenderer> MeshRenderers { get => renderers; }
    public List<Color> Colors { get => colors; }
}
