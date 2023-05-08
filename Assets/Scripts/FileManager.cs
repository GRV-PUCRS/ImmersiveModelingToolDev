using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    private static int fileCounter = 0;
    private static string fileManagerDataFile = "filemanager.txt";

    public static void LoadFileManagerData()
    {
        string currentFileCounterString = File.ReadAllText(Application.persistentDataPath + "/" + fileManagerDataFile);

        if (!currentFileCounterString.Equals(""))
            fileCounter = int.Parse(currentFileCounterString);
        else
            fileCounter = 0;
    }

    public static void SaveFileManagerData()
    {
        File.WriteAllText(Application.persistentDataPath + $"/{fileManagerDataFile}", fileCounter.ToString());
    }

    public static string CreateFile()
    {
        string newFileName = $"file_{fileCounter}.txt";
        File.Create(Application.persistentDataPath + $"/{newFileName}");

        SaveFileManagerData();

        return newFileName;
    }

    public static bool CheckIfFileExist(string filePath)
    {
        return File.Exists(filePath);
    }

    public static bool CheckIfFileExistInPersistentFolder(string fileName)
    {
        return CheckIfFileExist(Application.persistentDataPath + "/" + fileName);
    }

    public static void SaveDatainFile(string fileName, string fileContent)
    {
        File.WriteAllText(fileName, fileContent);
    }

    public static string ReadDataFromFile(string fileName)
    {
        return File.ReadAllText(Application.persistentDataPath + "/" + fileName);
    }
}
