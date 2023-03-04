using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("ButtonContainer")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button loadButton;

    [Header("LoadingMenu")]
    [SerializeField] private GameObject saveSlotMenu;
    [SerializeField] private GameObject emptyMenu;
    private void Start()
    {
        if (!DataManager.Instance.HasData())
        {
            continueButton.interactable = false;
            loadButton.interactable = false;
        }
        else
        {
            InitalizeSaveSlots();
        }
    }

    public void StartGame()
    {
        DataManager.Instance.CreateGame(10, 10);
        SceneManager.LoadSceneAsync(1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadLastGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


    public void InitalizeSaveSlots()
    {
        HashSet<string> saveSlots = DataManager.Instance.GetAllSaves();
        saveSlotMenu.GetComponent<SaveSlotMenu>().InitalizeAllSaveSlots(saveSlots);
    }

    public void DisableAllButtons()
    {
        continueButton.interactable = false;
        createButton.interactable = false;
        closeButton.interactable = false;
        loadButton.interactable = false;
    }
}
