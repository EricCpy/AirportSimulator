using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduelObject
{
    public enum FlightType
    {
        Takeoff,
        Landing
    }

    public DateTime time;
    public string vehicleType;
    public FlightType flightType;

    public ScheduelObject(DateTime time, string vehicleType, FlightType flightType)
    {
        this.time = time;
        this.vehicleType = vehicleType;
        this.flightType = flightType;
    }

    public ScheduelSaveObject ToScheduelSaveObject()
    {
        return new ScheduelSaveObject(time.ToString("o"), vehicleType, flightType);
    }

    public override string ToString() {
        return "Time: " + time + ", FlightType: " + flightType + ", VehicleType: " + vehicleType;
    }
}
