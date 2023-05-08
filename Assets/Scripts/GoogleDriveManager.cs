using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityGoogleDrive;
using static SessionManager;

public class GoogleDriveManager : Singleton<GoogleDriveManager>
{
    public TextMeshProUGUI log;

    private string currentServerID = "root";
    private string prefixLink = "https://drive.google.com/uc?export=download&id=";
    private string sessionDatasetLink = "https://drive.google.com/uc?export=download&id=1mg-JVzuxqxtmrv2m6GkRDXYcDMKYsVpb";

    private Dictionary<string, List<string>> filesFromServer;

    /*
    public void GetSessionFromServer(string serverName, Action<List<SessionFile>> resultCallback, Action<float> progressCallback, Action<string> errorCallback)
    {
        StartCoroutine(GetSessionFromFolder(serverName, resultCallback, progressCallback, errorCallback));
    }
    */

    public void GetSessionFromServer(string serverName, Action<List<SessionFile>> resultCallback, Action<float> progressCallback, Action<string> errorCallback)
    {
        StartCoroutine(GetSessionFromFolder(serverName, resultCallback, progressCallback, errorCallback));
    }

    IEnumerator GetSessionFromFolder(string serverName, Action<List<SessionFile>> resultCallback, Action<float> progressCallback, Action<string> errorCallback) 
    {
        filesFromServer = new Dictionary<string, List<string>>();

        yield return StartCoroutine(DownloadManager.Instance.DownloadStringFileFromURL(sessionDatasetLink, (string result) => {
            string[] lines = result.Replace("\n", "").Split(';');

            foreach (string line in lines)
            {
                string[] substring = line.Split(',');

                if (substring.Length != 2) continue;

                if (!filesFromServer.ContainsKey(substring[0]))
                    filesFromServer.Add(substring[0], new List<string>());

                filesFromServer[substring[0]].Add(substring[1]);
            }

            Debug.Log("Parse Session Data: " + filesFromServer.Values);
        }, null, null));

        Debug.Log("files " + filesFromServer.Count);
        foreach (string key in filesFromServer.Keys)
        {
            Debug.Log("   " + key);
        
            foreach (string value in filesFromServer[key])
            {
                Debug.Log("      " + value);
            }
        }

        if (!filesFromServer.ContainsKey(serverName))
        {
            Debug.Log("");
            resultCallback?.Invoke(new List<SessionFile>());
            
            yield break;
        }

        List<SessionFile> sessionsFiles = new List<SessionFile>();

        foreach (string session in filesFromServer[serverName])
        {
            string fileContent = "";

            Debug.Log(prefixLink + session);
            yield return StartCoroutine(DownloadManager.Instance.DownloadStringFileFromURL(prefixLink + session, (string result) => {
                fileContent = result;
            }, null, null));

            if (fileContent.Equals("")) continue;

            Plan plan = JsonUtility.FromJson<Plan>(fileContent);

            sessionsFiles.Add(new SessionFile(plan.Name, prefixLink + session, SessionFile.SessionFileState.Cloud, fileContent));

        }

        resultCallback?.Invoke(sessionsFiles);
    }

    /*
    IEnumerator GetSessionFromFolder(string serverName, Action<List<SessionFile>> resultCallback, Action<float> progressCallback, Action<string> errorCallback)
    {
        List<SessionFile> sessions = new List<SessionFile>();

        GoogleDriveFiles.ListRequest request = new GoogleDriveFiles.ListRequest();
        request.Fields = new List<string> { "files(id)" };
        request.Q = $"name = '{serverName}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";

        yield return request.Send();

        if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
        {
            log.text += "\nErro baixando arquivos: " + request.IsError + " " + request.Error;
            errorCallback?.Invoke($"Failed to retrieve '{serverName}'");
            yield break;
        }

        log.text += "\nBaixou pasta: ";
        currentServerID = request.ResponseData.Files[0].Id;

        List<string> filesIDs = new List<string>();
        GoogleDriveFiles.ListRequest list = GoogleDriveFiles.List();
        list.Fields = new List<string> { "files(id, name, parents)" };
        list.Q = "trashed = false";

        yield return list.Send();

        list.ResponseData.Files.ForEach(e => {
            if (e.Parents != null && e.Parents[0].Equals(currentServerID))
            {
                log.text += "\nAdd File: " + e.Name;
                filesIDs.Add(e.Id); 
                sessions.Add(new SessionFile(e.Name, e.Id, "", SessionFile.SessionFileState.Cloud, ""));
            }
        });


        int i = 0;
        foreach (string fileID in filesIDs)
        {
            GoogleDriveFiles.DownloadRequest r = GoogleDriveFiles.Download(fileID);

            log.text += "\nBaixando " + fileID + " ...";
            yield return r.Send();

            log.text += "\nBaixou";
            sessions[i].content = Encoding.UTF8.GetString(r.ResponseData.Content);
        }

        resultCallback?.Invoke(sessions);
        log.text += "\nFoi";
    }
    */


    public void CreateFileInDrive(SessionFile sessionFile, Action<SessionFile, string> callback)
    {
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File() { Name = sessionFile.name, Content = Encoding.UTF8.GetBytes(sessionFile.content) };
        newFile.Parents = new List<string> { currentServerID };

        GoogleDriveFiles.CreateRequest request = GoogleDriveFiles.Create(newFile);
        request.Send().OnDone += file => callback?.Invoke(sessionFile, file.Id);
    }

    public void UpdateFileInDrive(SessionFile sessionFile)
    {
        /*
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File() { Name = sessionFile.name, Content = Encoding.UTF8.GetBytes(sessionFile.content) };

        GoogleDriveFiles.UpdateRequest request = GoogleDriveFiles.Update(sessionFile.googleDriveId, newFile);
        request.Send().OnDone += files => Debug.Log("Foi!");
        */
    }
}
