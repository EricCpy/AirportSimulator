using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System.IO;

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
    [SerializeField] private TMP_InputField importInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button importButton;
    [Header("ScheduelSlotMenu")]
    [SerializeField] private ScheduelSlotMenu scheduelSlotMenu;

    void Start()
    {
        standardColor = currentSelected.GetComponent<Image>().color;
        SelectButton(currentSelected);
    }

    private void OnEnable()
    {
        InitalizeDropdownSelection();
    }

    private void InitalizeDropdownSelection()
    {
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
            if (inputDt <= ScheduelManager.Instance.airportTime) throw new Exception("Input Datetime too small");
            ScheduelObject.FlightType flightType = (ScheduelObject.FlightType)Enum.Parse(typeof(ScheduelObject.FlightType), flightTypeInput.captionText.text);
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

    private void OnDisable()
    {
        airplaneTypeInput.ClearOptions();
    }

    public void ImportScheduelEntries()
    {
        try
        {
            ScheduelJsonData data = JsonUtility.FromJson<ScheduelJsonData>(importInput.text);
            foreach (var arrival in data.arrivals)
            {
                DateTime time = DateTime.Parse(arrival.arrivalTime);
                if (time <= ScheduelManager.Instance.airportTime) continue;
                ScheduelManager.Instance.CreateNewScheduelEntry(time, arrival.airplaneType, ScheduelObject.FlightType.Landing);
            }
            foreach (var departure in data.departures)
            {
                DateTime time = DateTime.Parse(departure.departureTime);
                if (time <= ScheduelManager.Instance.airportTime) continue;
                ScheduelManager.Instance.CreateNewScheduelEntry(time, departure.airplaneType, ScheduelObject.FlightType.Takeoff);
            }
            importInput.text = "";
            importButton.GetComponent<Image>().color = standardColor;
        }
        catch (Exception e)
        {
            importButton.GetComponent<Image>().color = failColor;
            Debug.Log(e);
        }
    }

    public void OpenFile()
    {
        string path = EditorUtility.OpenFilePanel("Select Json (.json)", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                importInput.text = json;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
    }
}
