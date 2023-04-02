using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour, IData
{
    Dictionary<string, Vehicle> airplanes = new Dictionary<string, Vehicle>();
    public List<Vehicle> defaultAirplanes = new() { };
    public Vehicle bus;
    public Vehicle taxi;
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
            Debug.Log(airplane.ToVehicleSaveObject().capacity);
            data.vehicleObjects.Add(airplane.ToVehicleSaveObject());
        }
    }

    public Vehicle CreateNewAirplane(float speed, int capacity, string vehicleName, Color color) {
        Vehicle newAirplane = new Vehicle(speed, capacity, airplane, vehicleName, Vehicle.VehicleType.Airplane, color, 5, speed/5);
        airplanes.Add(newAirplane.vehicleName, newAirplane);
        return newAirplane;
    }

    public ICollection<Vehicle> GetAllAirplanes() {
        return airplanes.Values;
    }

    public ICollection<string> GetAllAirplaneNames() {
        return airplanes.Keys;
    }

    public void RemoveAirplanes(ICollection<Vehicle> airplaneList) {
        foreach(var airplane in airplaneList) {
            airplanes.Remove(airplane.vehicleName);
        }
    }

    public Vehicle GetAirplane(string airplaneType) {
        Vehicle vehicle = null;
        airplanes.TryGetValue(airplaneType, out vehicle);
        return vehicle;
    }
}
