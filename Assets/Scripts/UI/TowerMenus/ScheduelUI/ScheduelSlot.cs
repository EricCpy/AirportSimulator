using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduelSlot : MonoBehaviour
{
    [Header("InfoFields")]
    [SerializeField] private TextMeshProUGUI scheduelTime;
    [SerializeField] private TextMeshProUGUI scheduelVehicleName;
    [SerializeField] private TextMeshProUGUI scheduelFlightType;
    [Header("Color")]
    [SerializeField] private Color selectedColor;
    private string vehicleName;
    private ScheduelSlotMenu scheduelSlotMenu;
    private bool selected;
    private ScheduelObject scheduelEntry;
    private Color standardColor;
    public void InitalizeScheduelSlot(ScheduelObject scheduelEntry, ScheduelSlotMenu scheduelSlotMenu)
    {
        SetInfo(scheduelEntry);
        this.standardColor = gameObject.GetComponent<Image>().color;
        this.scheduelEntry = scheduelEntry;
        this.scheduelSlotMenu = scheduelSlotMenu;
    }

    public void SetInfo(ScheduelObject scheduelEntry)
    {
        scheduelTime.text = scheduelEntry.time.ToString();
        scheduelVehicleName.text = scheduelEntry.vehicleType;
        scheduelFlightType.text = scheduelEntry.flightType.ToString();
    }

    public void SelectSlot()
    {
        if(selected) {
            //Deselect
            gameObject.GetComponent<Image>().color = standardColor;
            scheduelSlotMenu.remove.Remove(scheduelEntry);
        } else {
            //Select
            gameObject.GetComponent<Image>().color = selectedColor;
            scheduelSlotMenu.remove.Add(scheduelEntry, this);
        }
        selected = !selected;
    }
}
