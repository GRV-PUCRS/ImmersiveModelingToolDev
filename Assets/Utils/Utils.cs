using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            GameObject.Destroy(child.gameObject);
        }
    }

    public static string GetFileNameFromPath(string filePath)
    {
        string[] fileStrings = filePath.Replace("\\", "/").Split('/');

        return fileStrings[fileStrings.Length - 1];
    }

    public static void DeleteFolderContent(string folderPath)
    {
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }
    }

    //https://answers.unity.com/questions/422472/how-can-i-compare-colliders-layer-to-my-layermask.html
    public static bool CheckLayerWithMask(LayerMask singleLayer, LayerMask layerMask)
    {
        return (1 << singleLayer & layerMask) != 0;
    }

    public static bool IsInteractableObject(GameObject gameObject)
    {
        return gameObject.layer.Equals(LayerMask.NameToLayer("InteractableObject"));
    }

    public static Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();

        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (var colider in colliders) bounds.Encapsulate(colider.bounds);

        return bounds;
    }
}