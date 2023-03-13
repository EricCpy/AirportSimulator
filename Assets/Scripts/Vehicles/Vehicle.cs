using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Vehicle : ScriptableObject
{
    public enum VehicleType
    {
        Car,
        Airplane
    }

    public float speed = 10f;
    public Transform prefab;
    public string vehicleName;
    public VehicleType type;
    public Color color;
    public Vehicle(float speed, Transform prefab, string vehicleName, VehicleType type, Color color)
    {
        this.speed = speed;
        this.prefab = prefab;
        this.vehicleName = vehicleName;
        this.type = type;
        this.color = color;
        prefab.GetComponent<SpriteRenderer>().color = color;
    }

    public Vehicle(VehicleSaveObject vehicleSaveObject, Transform prefab)
    {
        this.prefab = prefab;
        this.vehicleName = vehicleSaveObject.vehicleName;
        this.type = vehicleSaveObject.vehicleType;
        ColorUtility.TryParseHtmlString(vehicleSaveObject.color, out this.color);
        prefab.GetComponent<SpriteRenderer>().color = color;
    }

    public VehicleSaveObject ToVehicleSaveObject()
    {
        VehicleSaveObject vso = new VehicleSaveObject(speed,type,vehicleName,  ColorUtility.ToHtmlStringRGB(color));
        return vso;
    }
}
