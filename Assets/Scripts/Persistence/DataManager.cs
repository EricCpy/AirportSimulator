using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string fileName;
    private FileDataHandler dataHandler;
    private Data data;
    public static DataManager Instance {get; private set;}
    private List<IData> dataObjects;
    private void Awake() {
        if(Instance != null) {
            throw new UnityException("DataManager has already an Instance");
        }
        Instance = this;
    }

    public void Start() {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataObjects = FindAllDataObjects();
        LoadGame();
    }

    public void CreateGame() {
        data = new Data();
    }

    public void LoadGame() {
        data = dataHandler.Load();
        if(data ==  null) {
            CreateGame();
        }
        foreach(IData dataObject in dataObjects) {
            dataObject.LoadData(data);
        }
    }

    public void SaveGame() {
        foreach(IData dataObject in dataObjects) {
            dataObject.SaveData(ref data);
        }
        dataHandler.Save(data);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private List<IData> FindAllDataObjects() {
        IEnumerable<IData> dataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();
        return new List<IData>(dataObjects);
    }
}
