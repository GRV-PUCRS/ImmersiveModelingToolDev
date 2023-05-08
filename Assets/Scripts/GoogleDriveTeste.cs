using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class GoogleDriveTeste : MonoBehaviour
{
    public GoogleDriveManager gdm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            Teste();
        }
    }

    public void Teste()
    {
        gdm.GetSessionFromServer("Grv", null, null, null);
    }

    private void Teste2()
    {
        // Listing files.
        var list = GoogleDriveFiles.List();
        list.Fields = new List<string> { "nextPageToken, files(id, name, parents)" };
        list.Send().OnDone += fileList => fileList.Files.ForEach(e => {
            if (e.Parents != null)
                Debug.Log($"({e.Parents.Count}) {e.Parents[0]} - File: {e.Name}");
            else
                Debug.Log($"File: " + e.Name);
        });

        /*
        // Uploading a file.
        var file = new UnityGoogleDrive.Data.File { Name = "Image.png", Content = rawFileData };
        GoogleDriveFiles.Create(file).Send();

        // Downloading a file.
        GoogleDriveFiles.Download(fileId).Send().OnDone += file => ...;

        // All the requests are compatible with the .NET 4 asynchronous model.
        var aboutData = await GoogleDriveAbout.Get().Send();
        */
    }
}
