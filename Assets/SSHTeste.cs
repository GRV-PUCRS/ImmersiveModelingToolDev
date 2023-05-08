using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static SessionManager;

public class SSHTeste : MonoBehaviour
{
    /*
    public TextMeshProUGUI log;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            Debug.Log("Envia Arquivo!");
            StartCoroutine(FileUploadSFTP());
            //Teste();
        }
    }

    public void Teste()
    {
        StartCoroutine(Upload());

        //GoogleDriveManager.Instance.CreateFileInDrive(new SessionFile("arquivo_teste.txt", "", "", SessionFile.SessionFileState.Cloud, "Arquivo com texto"), null);
    }

    IEnumerator Upload()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("https://drive.google.com/drive/folders/1zPAoY4l-Zf3EX8By_7H-_z9qnRMS3tA2?usp=sharing", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    // you could pass the host, port, usr, pass, and uploadFile as parameters
    IEnumerator FileUploadSFTP()
    {
        var host = "sparta.pucrs.br";
        var port = 22;
        var username = @"portoalegre\17204102";
        var password = "Vi@4671251212";

        // path for file you want to upload
        var uploadFile = $"{Application.persistentDataPath}/Teste_2.txt";

        File.WriteAllText(uploadFile, "Teste FOI  AAAA");

        log.text += "\nCriando client ...";

        yield return null;

        try
        {
            using (var client = new SftpClient(host, port, username, password))
            {
                client.OperationTimeout = new TimeSpan(0, 0, 20);

                log.text += "\nConectando ...";
                client.Connect();
                if (client.IsConnected)
                {
                    log.text += "\nI'm connected to the client";

                    using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, "Drive-P-grv/sitegrv/AppAssistenciaGuiada/Teste_2.txt");
                        log.text += "\nFOI 1";
                    }

                    log.text += ("\nFOI 2");

                    client.Disconnect();
                }
                else
                {
                    log.text += ("\nI couldn't connect");
                }
            }

        }
        catch (Exception e)
        {
            log.text += ("\nErro: " + e.Message);
        }
    }

    public void FileUploadTeste()
    {
        StartCoroutine(Thread());
    }

    IEnumerator Thread()
    {
        yield return StartCoroutine(FileUploadSFTP());
    }

    private void OnEnable()
    {
        //InputController.Instance.OnFourButtonPressed.AddListener(Teste);
        InputController.Instance.OnFourButtonPressed.AddListener(FileUploadTeste);
    }
    */
}
