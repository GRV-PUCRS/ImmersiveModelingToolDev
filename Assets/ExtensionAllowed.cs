using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionAllowed
{
    public enum ExtensionValue { Text, Image, Video, Prefab };
    private static Dictionary<string, ExtensionValue> extensions = new Dictionary<string, ExtensionValue>
    {
        {"prefab", ExtensionValue.Prefab},
        {"png", ExtensionValue.Image},
        {"jpg", ExtensionValue.Image},
        {"txt", ExtensionValue.Text},
        {"avi", ExtensionValue.Video},
        {"mp4", ExtensionValue.Video},
    };

    public static bool IsAllowedExtension(string extension) => extensions.Keys.Any(s => s.Equals(extension));

    public static ExtensionValue GetExtensionValue(string extension)
    {
        return extensions[extension];
    }

    public static bool IsPrefab(string extension)
    {
        if (!extensions.ContainsKey(extension)) return false;

        return extensions[extension] == ExtensionValue.Prefab;
    }
}
