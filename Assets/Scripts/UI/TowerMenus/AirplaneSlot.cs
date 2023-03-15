using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneSlot : MonoBehaviour
{   
    [Header("InfoFields")]
    [SerializeField] private TextMeshProUGUI airplaneName;
    [SerializeField] private TextMeshProUGUI airplaneSpeed;
    [SerializeField] private TextMeshProUGUI airplaneCapacity;
    [Header("Color")]
    [SerializeField] private Color selectedColor;
    private string vehicleName;
    private AirplaneSlotMenu airplaneSlotMenu;
    private bool selected;
    private Vehicle vehicle;
    public void InitalizeAirplaneSlot(Vehicle vehicle, AirplaneSlotMenu airplaneSlotMenu)
    {
        SetInfo(vehicle);
        SetColor(vehicle.color);
        this.vehicle = vehicle;
        this.airplaneSlotMenu = airplaneSlotMenu;
    }

    public void SetInfo(Vehicle vehicle)
    {
        airplaneName.text = vehicle.vehicleName;
        airplaneSpeed.text = vehicle.speed + "m/s";
        airplaneCapacity.text = vehicle.capacity + " Passengers";
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<Image>().color = color;
    }

    public void SelectSlot()
    {
        if(selected) {
            //Deselect
            SetColor(vehicle.color);
            airplaneSlotMenu.remove.Remove(vehicle);
        } else {
            //Select
            gameObject.GetComponent<Image>().color = selectedColor;
            airplaneSlotMenu.remove.Add(vehicle, this);
        }
        selected = !selected;
    }
}
