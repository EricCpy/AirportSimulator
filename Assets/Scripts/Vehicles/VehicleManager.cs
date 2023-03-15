using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour, IData
{
    Dictionary<string, Vehicle> airplanes = new Dictionary<string, Vehicle>();
    public List<Vehicle> defaultAirplanes = new() { };
    [SerializeField] private Vehicle bus;
    [SerializeField] private Vehicle taxi;
    [SerializeField] private Transform airplane;
    public static VehicleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("VehicleManager has already an Instance");
        }
        Instance = this;
    }

    public void LoadData(Data data)
    {
        if (data.vehicleObjects.Count == 0)
        {
            foreach (var airplane in defaultAirplanes)
            {
                airplanes.Add(airplane.vehicleName, airplane);
            }
        }
        else
        {
            foreach (var airplane in data.vehicleObjects)
            {
                airplanes.Add(airplane.vehicleName, new Vehicle(airplane, this.airplane));
            }
        }
    }

    public void SaveData(Data data)
    {
        foreach (var airplane in airplanes.Values)
        {
            data.vehicleObjects.Add(airplane.ToVehicleSaveObject());
        }
    }

    public void CreateNewAirplane(float speed, int capacity, string vehicleName, Color color) {
        Vehicle newAirplane = new Vehicle(speed, capacity, airplane, vehicleName, Vehicle.VehicleType.Airplane, color);
        airplanes.Add(newAirplane.vehicleName, newAirplane);
    }

    public ICollection<Vehicle> GetAllAirplanes() {
        return airplanes.Values;
    }

    public void RemoveAirplanes(ICollection<Vehicle> airplaneList) {
        foreach(var airplane in airplaneList) {
            airplanes.Remove(airplane.vehicleName);
        }
    }
}
