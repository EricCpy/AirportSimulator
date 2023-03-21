using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScheduelSaveObject
{
    public string time;
    public Vehicle.VehicleType vehicleType;
    public ScheduelObject.FlightType flightType;
    public ScheduelSaveObject(string time, Vehicle.VehicleType vehicleType, ScheduelObject.FlightType flightType)
    {
        this.time = time;
        this.vehicleType = vehicleType;
        this.flightType = flightType;
    }
}
