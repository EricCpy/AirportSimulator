using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScheduelUI : MonoBehaviour
{
    [Header("ButtonFields")]
    [SerializeField] private Button currentSelected;
    private Color standardColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color failColor;
    [Header("InputFields")]
    [SerializeField] private TMP_InputField flightDateInput;
    [SerializeField] private TMP_InputField flightTimeInput;
    [SerializeField] private TMP_Dropdown flightTypeInput;
    [SerializeField] private TMP_Dropdown airplaneTypeInput;
    [SerializeField] private Button createButton;
    [Header("ScheduelSlotMenu")]
    [SerializeField] private ScheduelSlotMenu scheduelSlotMenu;

    void Start()
    {
        standardColor = currentSelected.GetComponent<Image>().color;
        SelectButton(currentSelected);
        InitalizeScheduelSlots();
    }

    private void OnEnable() {
        InitalizeDropdownSelection();
    }

    private void InitalizeDropdownSelection() {
        ICollection<string> names = VehicleManager.Instance.GetAllAirplaneNames();
        airplaneTypeInput.AddOptions(names.ToList());
    }

    public void CreateFlight()
    {
        try
        {
            string[] date = flightDateInput.text.Split(".");
            int day = int.Parse(date[0]);
            int month = int.Parse(date[1]);
            int year = int.Parse(date[2]);
            string[] time = flightTimeInput.text.Split(".");
            int hour = int.Parse(time[0]);
            int minute = int.Parse(time[1]);
            DateTime inputDt = new DateTime(year, month, day, hour, minute, 0);
            createButton.GetComponent<Image>().color = standardColor;
            ScheduelObject.FlightType flightType = (ScheduelObject.FlightType) Enum.Parse(typeof(ScheduelObject.FlightType), flightTypeInput.captionText.text);
            string airplane = airplaneTypeInput.captionText.text;
            ScheduelObject scheduelEntry = ScheduelManager.Instance.CreateNewScheduelEntry(inputDt, airplane, flightType);
            scheduelSlotMenu.CreateScheduelSlot(scheduelEntry);
            createButton.GetComponent<Image>().color = standardColor;
        }
        catch (Exception e)
        {
            createButton.GetComponent<Image>().color = failColor;
            Debug.Log(e);
        }
    }

    public void ValidateDate()
    {
        try
        {
            string[] date = flightDateInput.text.Split(".");
            int day = int.Parse(date[0]);
            int month = int.Parse(date[1]);
            int year = int.Parse(date[2]);
            string[] time = flightTimeInput.text.Split(".");
            DateTime inputDate = new DateTime(year, month, day);
        }
        catch (Exception e)
        {
            createButton.GetComponent<Image>().color = failColor;
            Debug.Log(e);
        }
        createButton.GetComponent<Image>().color = standardColor;
    }

    public void ValidateTime()
    {
        try
        {
            string[] time = flightTimeInput.text.Split(".");
            int hour = int.Parse(time[0]);
            int minute = int.Parse(time[1]);
            DateTime inputDate = new DateTime(1, 1, 1, hour, minute, 0);
        }
        catch (Exception e)
        {
            createButton.GetComponent<Image>().color = failColor;
            Debug.Log(e);
        }
        createButton.GetComponent<Image>().color = standardColor;
    }

    public void SelectButton(Button button)
    {
        currentSelected.GetComponent<Image>().color = standardColor;
        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }

    public void RemoveScheduelEntries()
    {
        scheduelSlotMenu.RemoveScheduelEntries();
    }

    private void InitalizeScheduelSlots()
    {
        ICollection<ScheduelObject> scheduelEntries = ScheduelManager.Instance.GetAllScheduelEntries();
        scheduelSlotMenu.InitalizeScheduelSlots(scheduelEntries);
    }

    private void OnDisable() {
        airplaneTypeInput.ClearOptions();
    }

    public void ImportScheduelEntries()
    {
        //TODO
    }
}
