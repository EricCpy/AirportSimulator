using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{

    public enum ButtonType
    {
        None,
        Hand,
        Trashcan,
        Planestop,
        Hangar,
        Road,
        Watchtower,
        Terminal
    }
    private ButtonType[] buttonTypes = new ButtonType[] { ButtonType.None,
                                                          ButtonType.Hand,
                                                          ButtonType.Trashcan,
                                                          ButtonType.Planestop,
                                                          ButtonType.Hangar,
                                                          ButtonType.Road,
                                                          ButtonType.Watchtower,
                                                          ButtonType.Terminal };
    [SerializeField] private int[] timeScaleSteps = new int[] { 1, 2, 5, 10, 20 };
    private int currentTimeScale = 0;
    [SerializeField] private Button currentSelected;
    private CameraController cameraController;
    [SerializeField] private Color standardColor;
    [SerializeField] TMP_Text rotationText;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Button defaultButton;
    [SerializeField] private Button optionsMenuButton;
    [SerializeField] private GameObject airportMenu;

    [Header("Hangar")]
    [SerializeField] private GameObject hangarMenu;
    [SerializeField] private GameObject hangarCreationMenu;
    [SerializeField] private GameObject hangarGeneralMenu;
    public static InGameUI Instance { get; private set; }

    private void Awake() {
        if (Instance != null)
        {
            throw new UnityException("InGameUI has already an Instance");
        }
        Instance = this;
    }
    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        ActivateButton();
    }

    public void TimeScaleClick(GameObject button)
    {
        currentTimeScale++;
        if (currentTimeScale == timeScaleSteps.Length)
        {
            currentTimeScale = 0;
        }
        Time.timeScale = timeScaleSteps[currentTimeScale];
        button.GetComponentInChildren<TMP_Text>().text = timeScaleSteps[currentTimeScale] + "x";
    }

    public void SelectButton(Button button)
    {
        DeactivateAllButtons();
        currentSelected = button;
        ActivateButton();
    }

    public void ChangeButtonType(int id)
    {
        ButtonType bType = buttonTypes[id];
        if (bType == ButtonType.Hand)
        {
            cameraController.moveWithMouse = true;
        }
        else if (bType == ButtonType.Trashcan)
        {
            BuildingSystem.Instance.deletionMode = true;
        }
        else if (bType == ButtonType.None)
        {
            OptionsMenuReload();
        }
        else
        {
            //setze Building var auf objekt
            BuildingSystem.Instance.SetObjectType(bType);
        }
    }

    private void DeactivateAllButtons()
    {
        optionsMenu.SetActive(false);
        currentSelected.GetComponent<Image>().color = standardColor;
        cameraController.moveWithMouse = false;
        BuildingSystem.Instance.deletionMode = false;
        BuildingSystem.Instance.SetObjectType(ButtonType.None);
    }

    private void ActivateButton()
    {
        currentSelected.GetComponent<Image>().color = new Color(0, 0, 0, standardColor.a); //mache ausgewählten Button schwarz
    }

    public void Rotate()
    {
        rotationText.transform.Rotate(new Vector3(0, 0, -90));
        BuildingSystem.Instance.RotateAsset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.uiOpen && !optionsMenu.activeSelf) return;
            ChangeButtonType(0);
        }

        if (Input.GetKeyDown(KeyCode.R) && !GameManager.Instance.uiOpen)
        {
            Rotate();
        }
    }

    public void SaveAndExit()
    {
        DataManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync(0);
    }

    public void DeleteAirport()
    {
        DataManager.Instance.DeleteGame(DataManager.Instance.GetSelectedGameId());
        SceneManager.LoadSceneAsync(0);
    }

    public void OptionsMenuReload()
    {
        if (optionsMenu.activeSelf)
        {
            SelectButton(defaultButton);
            ChangeButtonType(1);
            optionsMenu.SetActive(false);
            GameManager.Instance.uiOpen = false;
        }
        else
        {
            SelectButton(optionsMenuButton);
            optionsMenu.SetActive(true);
            GameManager.Instance.uiOpen = true;
        }
    }

    public void OpenAirportManagerUI() {
        airportMenu.SetActive(true);
        GameManager.Instance.uiOpen = true;
    }

    public void OpenHangarCreationUI() {
        hangarMenu.SetActive(true);
        hangarGeneralMenu.SetActive(false);
        hangarCreationMenu.SetActive(true);
        GameManager.Instance.uiOpen = true;
    }

    public void OpenHangarGeneralUI() {
        hangarMenu.SetActive(true);
        hangarCreationMenu.SetActive(false);
        hangarGeneralMenu.SetActive(true);
        GameManager.Instance.uiOpen = true;
    }

    public void ToggleGrid() {
        BuildingSystem.Instance.ToggleGrid();
    }
}
