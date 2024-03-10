using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class JsonDataService : IDataService
{
    public bool SaveData<T>(string relativePath, T data)
    {
        string path = Application.persistentDataPath + relativePath;
        try
        {
            Debug.Log("Saving data...");
            if (File.Exists(path))
                File.Delete(path);
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            Debug.Log("Data saved.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message} {e.StackTrace} ");
            return false;
        }
    }
    public T LoadData<T>(string relativePath)
    {
        string path = Application.persistentDataPath + relativePath;
        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist.");
            throw new FileNotFoundException($"{path} does not exist.");
        }
        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message} {e.StackTrace} ");
            throw e;
        }
    }
}