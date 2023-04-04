using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption = false;
    [SerializeField] private bool initDataFromScene = false;
    private FileDataHandler dataHandler;
    private Data data;
    public static DataManager Instance { get; private set; }
    private List<IData> dataObjects;
    private string selectedGameId = "";
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("DataManager already existing.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        selectedGameId = PlayerPrefs.GetString("LastSaveSlotID", "test");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataObjects = FindAllDataObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Scene Unloaded");
        //Falls noch mehr Szenen hinzugefügt werden:
        //Scene currentScene = SceneManager.GetActiveScene();
        //if (currentScene.buildIndex != 0) SaveGame();
    }

    public void CreateGame(int width, int height)
    {
        data = new Data(width, height, System.DateTime.Now);
        PlayerPrefs.SetString("LastSaveSlotID", selectedGameId);
    }

    public void LoadGame()
    {
        Data tmpData = dataHandler.Load(selectedGameId);
        if (tmpData == null && data == null)
        {
            if (initDataFromScene)
            {
                CreateGame(10, 10);
            }
            else return;
        }

        if (tmpData != null) data = tmpData;
        foreach (IData dataObject in dataObjects)
        {
            try {
                dataObject.LoadData(data);
            } catch(System.Exception e) {
                Debug.Log(e);
            }
        }
        data.Clear();
    }

    public void SaveGame()
    {
        if (data == null) return;
        //finde nochmal alle Objekte, weil neue erzeugt werden können
        dataObjects = FindAllDataObjects();
        foreach (IData dataObject in dataObjects)
        {
            dataObject.SaveData(data);
        }
        dataHandler.Save(data, selectedGameId);
    }

    public void DeleteGame(string id)
    {
        dataHandler.Delete(id);
        data = null;
        initDataFromScene = false;
    }

    private void OnApplicationQuit()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex != 0) SaveGame();
    }

    private List<IData> FindAllDataObjects()
    {
        IEnumerable<IData> dataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();
        return new List<IData>(dataObjects);
    }

    public bool HasData()
    {
        return data != null;
    }

    public HashSet<string> GetAllSaves()
    {
        return dataHandler.GetAllLoadDirectories();
    }

    public void SetSelectedGameId(string id)
    {
        selectedGameId = id;
    }

    public string GetSelectedGameId()
    {
        return selectedGameId;
    }
}
