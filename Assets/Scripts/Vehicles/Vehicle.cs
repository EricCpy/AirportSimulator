using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
