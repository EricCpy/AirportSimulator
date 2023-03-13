using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleSaveObject
{
    public string vehicleName;
    public Vehicle.VehicleType vehicleType;
    public float speed;
    public string color;
    public VehicleSaveObject(float speed, Vehicle.VehicleType vehicleType, string vehicleName, string color)
    {
        this.speed = speed;
        this.vehicleType = vehicleType;
        this.vehicleName = vehicleName;
        this.color = color;
    }
}
