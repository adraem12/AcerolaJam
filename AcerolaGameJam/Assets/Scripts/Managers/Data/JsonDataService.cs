using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

public class JsonDataService : IDataService
{
    private const string key = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg6l1DVdwN8=";
    private const string iv = "JZuM0HQsWSBVpRHTeRZMYQ==";

    public bool SaveData<T>(string relativePath, T data)
    {
        string path = Application.persistentDataPath + relativePath;
        try
        {
            Debug.Log("Saving data...");
            if (File.Exists(path))
                File.Delete(path);
            using FileStream stream = File.Create(path);
            WriteEncryptedData(data, stream);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message} {e.StackTrace} ");
            return false;
        }
    }

    void WriteEncryptedData<T>(T data, FileStream fileStream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(key);
        aesProvider.IV = Convert.FromBase64String(iv);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new (fileStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
        Debug.Log("Data saved.");
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
            T data = ReadEncryptedData<T>(path);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message} {e.StackTrace} ");
            throw e;
        }
    }

    T ReadEncryptedData<T>(string path)
    {
        byte[] fileBytes = File.ReadAllBytes(path);
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(key);
        aesProvider.IV = Convert.FromBase64String(iv);
        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
        using MemoryStream decryptionStream = new (fileBytes);
        using CryptoStream cryptoStream = new (decryptionStream, cryptoTransform, CryptoStreamMode.Read);
        using StreamReader reader = new (cryptoStream);
        string result = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(result);
    }
}