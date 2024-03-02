using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public static class SimpleJsonReadJson
{
    public static string ReadData(string fileName)
    {
        string readData;
        string fileUrl = Application.streamingAssetsPath + "/" + fileName;

        using (StreamReader sr = new StreamReader(fileUrl))
        {
            readData = sr.ReadToEnd();
            sr.Close();
        }

        return readData;
    }

    public static void WriteData(string fileName, string writeData)
    {
        string fileUrl = Application.streamingAssetsPath + "/" + fileName;

        using (StreamWriter sw = new StreamWriter(fileUrl))
        {
            sw.Write(writeData);
            sw.Close();
        }
    }
}
