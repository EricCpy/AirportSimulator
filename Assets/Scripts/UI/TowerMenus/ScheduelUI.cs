using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduelUI : MonoBehaviour
{
    [Header("ButtonFields")]
    [SerializeField] private Button currentSelected;
    private Color standardColor;
    [SerializeField] private Color selectedColor;
    [Header("InputFields")]
    [SerializeField] private TMP_InputField flightDateInput;
    [SerializeField] private TMP_InputField flightTimeInput;
    [SerializeField] private TMP_Dropdown flightTypeInput;
    [SerializeField] private TMP_Dropdown airplaneTypeInput;
    void Start()
    {
        standardColor = currentSelected.GetComponent<Image>().color;
        SelectButton(currentSelected); 
    }

    public void CreateFlight()
    {
        //TODO
        Debug.Log("Create");
    }

    public void RemoveFlight()
    {
        //TODO
    }

    public void ImportFlights()
    {
        //TODO
    }

    public bool ValidateDate() {
        //TODO
        //GetCurrentTime
        //String.
        return true;
    }

    public bool ValidateTime() {
        //TODO
        return true;
    }
    public void SelectButton(Button button)
    {
        currentSelected.GetComponent<Image>().color = standardColor;
        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }
}
