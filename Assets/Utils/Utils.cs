using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void SetLayer(Transform obj, string layerName, bool toChilds)
    {
        obj.gameObject.layer = LayerMask.NameToLayer(layerName);

        if (!toChilds) return;

        foreach (Transform child in obj.transform)
            SetLayer(child, layerName, true);
    }

    public static void ClearChilds(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static string GetFileNameFromPath(string filePath)
    {
        string[] fileStrings = filePath.Replace("\\", "/").Split('/');

        return fileStrings[fileStrings.Length - 1];
    }
}
