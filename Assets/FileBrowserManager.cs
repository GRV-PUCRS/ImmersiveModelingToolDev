
//using AnotherFileBrowser.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Documentacao! https://github.com/SrejonKhan/AnotherFileBrowser
public class FileBrowserManager : MonoBehaviour
{
    public static void OpenFileBrowser(Action<string> callback)
    {/*
        var bp = new BrowserProperties();
        bp.filter = null;
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            callback?.Invoke(path);
        });*/
    }

    public static void OpenFolderBrowser(Action<string> callback)
    {/*
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
        bp.filterIndex = 0;

        new FileBrowser().OpenFolderBrowser(bp, path =>
        {
            callback?.Invoke(path);
        });*/
    }
}
