using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : Singleton<DownloadManager>
{
    public IEnumerator DownloadStringFileFromURL(string file, Action<string> resultCallback, Action<float> progressCallback, Action<string> errorCallback)
    {
        Debug.Log("==> " + file); 
        UnityWebRequest www = new UnityWebRequest(file);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            errorCallback?.Invoke(www.error);

            yield break;
        }

        resultCallback?.Invoke(www.downloadHandler.text);
    }

    public IEnumerator DonwloadAssetBundleURL(string url, Action<AssetBundle> resultCallback, Action<float> progressCallback, Action<string> errorCallback)
    {
        WWW www = new WWW(url);

        yield return www;

        if (!www.isDone)
        {
            Debug.Log(www.error);
            errorCallback?.Invoke(www.error);

            yield break;
        }

        resultCallback?.Invoke(www.assetBundle);
    }

    public IEnumerator UpdateLoadScreenSlider(UnityWebRequest request, Action<float> progressCallback)
    {
        while (!request.isDone)
        {
            progressCallback?.Invoke(request.downloadProgress);
            yield return new WaitForSeconds(.1f);
        }

        progressCallback?.Invoke(1f);
    }

    public void DownloadStringFile(string url, Action<string> callback)
    {
        StartCoroutine(DownloadStringFileFromURL(url, callback, null, null));
    }

    public void DownloadAssetBundle(string url, Action<AssetBundle> callback)
    {
        StartCoroutine(DonwloadAssetBundleURL(url, callback, null, null));
    }

    public void DownloadAssetBundle(List<string> url, Action<List<WWW>> callback)
    {
        StartCoroutine(DownloadAssetBundles(url, callback));
    }

    public void DownloadFiles(List<string> url, Action<Dictionary<string, byte[]>> callback)
    {
        StartCoroutine(DowloadBytes(url, callback));
    }

    private IEnumerator DownloadAssetBundles(List<string> urls, Action<List<WWW>> callback)
    {
        List<WWW> requests = new List<WWW>();
        Debug.Log("Baixando .... ");
        foreach (string url in urls)
        {
            WWW www = new WWW(url);

            yield return www;
            
            if (!www.isDone || www.assetBundle == null)
            {
                Debug.Log("Erro ao baixar objeto: " + url);
                continue;
            }
            Debug.Log("  " + www.assetBundle.name + " baixado com sucesso!");
            requests.Add(www);
        }

        callback?.Invoke(requests);
    }

    private IEnumerator DowloadBytes(List<string> urls, Action<Dictionary<string, byte[]>> callback)
    {
        Dictionary<string, byte[]> requests = new Dictionary<string, byte[]>();
        
        Debug.Log("Baixando .... ");

        foreach (string url in urls)
        {
            string[] tokens = url.Replace("\\", "/").Split('/');
            string name = tokens[tokens.Length - 1];

            UnityWebRequest www = UnityWebRequest.Get(url);

            yield return www.SendWebRequest();

            if (!www.isDone)
            {
                Debug.Log(" ERRO  " + name + "   " + url);
                continue;
            }

            Debug.Log("  " + name +"  " + url + " baixado com sucesso!");
            requests.Add(name, www.downloadHandler.data);
        }

        callback?.Invoke(requests);
    }
}