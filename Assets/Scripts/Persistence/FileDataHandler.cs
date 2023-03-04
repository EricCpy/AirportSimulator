using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler : MonoBehaviour
{
    private string dataPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataPath, string dataFileName)
    {
        this.dataPath = dataPath;
        this.dataFileName = dataFileName;
    }

    public Data Load()
    {
        string path = Path.Combine(dataPath, dataFileName);
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
                loadData = JsonUtility.FromJson<Data>(loadingData);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        return loadData;
    }

    public void Save(Data data)
    {
        //path combine, weil system abhängig / oder \
        string path = Path.Combine(dataPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string storingData = JsonUtility.ToJson(data, true);
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
}
