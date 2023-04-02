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

    public enum VehicleRotation
    {
        Left,
        Right,
        Up,
        Down
    }

    public float speed = 10f;
    public float sensorLength = 10f;
    public float accelerationSpeed = 5f;
    //capacity in passenger seats
    public int capacity = 10;
    public Transform prefab;
    public string vehicleName;
    public VehicleType type;
    public Color color;
    public Vehicle(float speed, int capacity, Transform prefab, string vehicleName, VehicleType type, Color color, float sensorLength, float accelerationSpeed)
    {
        this.speed = speed;
        this.capacity = capacity;
        this.prefab = prefab;
        this.vehicleName = vehicleName;
        this.type = type;
        this.color = color;
        this.sensorLength = sensorLength;
        this.accelerationSpeed = accelerationSpeed;
        prefab.GetComponentInChildren<SpriteRenderer>().color = color;
    }

    public Vehicle(VehicleSaveObject vehicleSaveObject, Transform prefab)
    {
        this.prefab = prefab;
        this.speed = vehicleSaveObject.speed;
        this.capacity = vehicleSaveObject.capacity;
        this.sensorLength = vehicleSaveObject.sensorLength;
        this.accelerationSpeed = vehicleSaveObject.accelerationSpeed;
        this.vehicleName = vehicleSaveObject.vehicleName;
        this.type = vehicleSaveObject.vehicleType;
        ColorUtility.TryParseHtmlString("#" + vehicleSaveObject.color, out this.color);
        prefab.GetComponentInChildren<SpriteRenderer>().color = color;
    }

    public VehicleSaveObject ToVehicleSaveObject()
    {
        VehicleSaveObject vso = new VehicleSaveObject(speed,capacity,type,vehicleName, ColorUtility.ToHtmlStringRGB(color), sensorLength, accelerationSpeed);
        return vso;
    }

    public Quaternion GetRotation(VehicleRotation rot)
    {
        switch (rot)
        {
            default:
            case VehicleRotation.Down: return Quaternion.Euler(0,0,90);
            case VehicleRotation.Left: return Quaternion.identity;
            case VehicleRotation.Up: return Quaternion.Euler(0,0,270);
            case VehicleRotation.Right: return Quaternion.Euler(180,0,180);
        }
    }

}
