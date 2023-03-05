using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
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

    [Header("CreationMenu InputFields")]
    [SerializeField] private TMP_InputField airportName;
    [SerializeField] private TMP_InputField height;
    [SerializeField] private TMP_InputField width;
    private void Start()
    {
        if (!DataManager.Instance.HasData())
        {
            continueButton.interactable = false;
        }
        if (!InitalizeSaveSlots()) loadButton.interactable = false;
    }

    public void CreateGame()
    {
        try
        {
            int creationHeight = 10;
            int creationWidth = 10;
            if (!height.text.Equals("")) creationHeight = Mathf.Min(Mathf.Max(Mathf.Abs(int.Parse(height.text)), 10), 1000);
            if (!width.text.Equals("")) creationWidth = Mathf.Min(Mathf.Max(Mathf.Abs(int.Parse(width.text)), 10), 1000);
            DataManager.Instance.SetSelectedGameId(airportName.text);
            DataManager.Instance.CreateGame(creationWidth, creationHeight);
            SceneManager.LoadSceneAsync(1);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadLastGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


    public bool InitalizeSaveSlots()
    {
        HashSet<string> saveSlots = DataManager.Instance.GetAllSaves();
        saveSlotMenu.GetComponent<SaveSlotMenu>().InitalizeAllSaveSlots(saveSlots);
        return saveSlots.Count > 0;
    }

    public void DisableAllButtons()
    {
        continueButton.interactable = false;
        createButton.interactable = false;
        closeButton.interactable = false;
        loadButton.interactable = false;
    }
}
