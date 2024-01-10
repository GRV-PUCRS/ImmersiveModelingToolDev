using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionAllowed
{
    public enum ExtensionValue { Text, Image, Video, CustomText };

    private static Dictionary<string, ExtensionValue> extensions = new Dictionary<string, ExtensionValue>
    {
        {"png", ExtensionValue.Image},
        {"jpg", ExtensionValue.Image},
        {"txt", ExtensionValue.Text},
        {"avi", ExtensionValue.Video},
        {"mp4", ExtensionValue.Video},
        {"ptx", ExtensionValue.CustomText},
    };

    public static bool IsAllowedExtension(string fileName) => extensions.Keys.Any(s => fileName.EndsWith(s));

    public static ExtensionValue GetExtensionValue(string extension)
    {
        return extensions[extension];
    }
}
