using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if(!DataManager.Instance.HasData()) {
            continueButton.interactable = false;
        }
    }

    public void StartGame()
    {
        DataManager.Instance.CreateGame();
        SceneManager.LoadSceneAsync(1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadGame()
    {

    }
}
