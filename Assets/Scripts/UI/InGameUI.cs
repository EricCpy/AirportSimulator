using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        Watchtower
    }
    private ButtonType[] buttonTypes = new ButtonType[] { ButtonType.None,
                                                          ButtonType.Hand,
                                                          ButtonType.Trashcan,
                                                          ButtonType.Planestop,
                                                          ButtonType.Hangar,
                                                          ButtonType.Road,
                                                          ButtonType.Watchtower };
    [SerializeField] private int[] timeScaleSteps = new int[] { 1, 2, 5, 10, 20 };
    private int currentTimeScale = 0;
    [SerializeField] private Button currentSelected;
    private CameraController cameraController;
    [SerializeField] private Color standardColor;
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
        print(button.name);
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
            //setze var zum clearen, wenn man auf feld drückt
            cameraController.moveWithMouse = true;
        }
        else if (bType == ButtonType.Trashcan)
        {
            //setze var zum clearen, wenn man auf feld drückt
        }
        else if (bType == ButtonType.None)
        {
            //öffne Settingsmenü
        }
        else
        {
            //setze Building var auf objekt
        }
    }

    private void DeactivateAllButtons()
    {
        currentSelected.GetComponent<Image>().color = standardColor;
        cameraController.moveWithMouse = false;
    }

    private void ActivateButton()
    {
        currentSelected.GetComponent<Image>().color = new Color(0, 0, 0, standardColor.a); //mache ausgewählten Button schwarz
    }
}