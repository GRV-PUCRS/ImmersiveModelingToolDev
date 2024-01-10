using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ServerCacheData
{
    [SerializeField] private string serverID;
    [SerializeField] private string serverPassword;
    [SerializeField] private string sessionsFolderPath;
    [SerializeField] private string objectsFolderPath;

    public ServerCacheData()
    {
        serverID = "";
        serverPassword = "";
        sessionsFolderPath = "";
        objectsFolderPath = "";
    }

    public string ServerID { get => serverID; set => serverID = value; }
    public string ServerPassword { get => serverPassword; set => serverPassword = value; }
    public string SessionsFolderPath { get => sessionsFolderPath; set => sessionsFolderPath = value; }
    public string ObjectsFolderPath { get => objectsFolderPath; set => objectsFolderPath = value; }
}
