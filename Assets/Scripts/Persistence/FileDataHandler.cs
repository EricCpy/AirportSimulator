using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileDataHandler
{
    private string dataPath = "";
    private string dataFileName = "";
    private bool encryption = false;
    private readonly string encryptionSecret = "l985sp";
    public FileDataHandler(string dataPath, string dataFileName, bool encryption)
    {
        this.dataPath = dataPath;
        this.dataFileName = dataFileName;
        this.encryption = encryption;
    }

    public Data Load(string id)
    {
        string path = Path.Combine(dataPath, id, dataFileName);
        Data loadData = null;
        if (File.Exists(path))
        {
            try
            {
                string loadingData = "";
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadingData = reader.ReadToEnd();
                    }
                }
                if (encryption)
                {
                    loadingData = XOREncrypt(loadingData);
                }

                loadData = JsonUtility.FromJson<Data>(loadingData);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        return loadData;
    }

    public void Save(Data data, string id)
    {
        //path combine, weil system abhängig / oder \
        string path = Path.Combine(dataPath, id, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string storingData = JsonUtility.ToJson(data, true);

            if (encryption)
            {
                storingData = XOREncrypt(storingData);
            }
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(storingData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    private string XOREncrypt(string data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sb.Append((char)(data[i] ^ encryptionSecret[i % encryptionSecret.Length]));
        }
        return sb.ToString();
    }
}
