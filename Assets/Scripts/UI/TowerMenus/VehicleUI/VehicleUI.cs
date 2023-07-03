using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleUI : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private TMP_InputField vehicleNameInput;
    [SerializeField] private TMP_InputField speedInput;
    [SerializeField] private TMP_InputField capacityInput;
    [SerializeField] private Image colorInput;
    [SerializeField] private Transform colorPointer;
    [Header("AirplaneSlotMenu")]
    [SerializeField] private AirplaneSlotMenu airplaneSlotMenu;
    public Color vehicleColor = Color.blue;
    private void Start()
    {
        InitalizeAirplaneSlots();
    }

    public void CreateAirplane()
    {
        try
        {
            float speed = 10;
            int capacity = 10;
            if (!speedInput.text.Equals("")) speed = Mathf.Min(Mathf.Max(Mathf.Abs(float.Parse(speedInput.text)), 10), 50);
            if (!capacityInput.text.Equals("")) capacity = Mathf.Min(Mathf.Max(Mathf.Abs(int.Parse(capacityInput.text)), 20), 300);
            Vehicle vehicle = VehicleManager.Instance.CreateNewAirplane(speed, capacity, vehicleNameInput.text, vehicleColor);
            airplaneSlotMenu.CreateAirplaneSlot(vehicle);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void RemoveAirplanes()
    {
        airplaneSlotMenu.RemoveAirplanes();
    }

    public void PickColor()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(Input.mousePosition);
        Vector2 imagePoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorInput.rectTransform, screenPoint, Camera.main, out imagePoint);
        Vector2 colorPointerPosition = imagePoint;
        imagePoint += colorInput.rectTransform.sizeDelta / 2; 
        Texture2D texture = colorInput.sprite.texture;
        Vector2Int point = new Vector2Int(
            (int)((texture.width * imagePoint.x) / colorInput.rectTransform.sizeDelta.x),
            (int)((texture.height * imagePoint.y) / colorInput.rectTransform.sizeDelta.y));
        Color color = texture.GetPixel(point.x, point.y);
        if(color.a > 0) {
            vehicleColor = texture.GetPixel(point.x, point.y);
            colorPointer.transform.localPosition = colorPointerPosition;
        }
    }

    private void InitalizeAirplaneSlots()
    {
        ICollection<Vehicle> airplanes = VehicleManager.Instance.GetAllAirplanes();
        airplaneSlotMenu.InitalizeAirplaneSlots(airplanes);
    }
}
