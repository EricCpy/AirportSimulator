using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportManager : MonoBehaviour, IData
{
    //Dict aller Flugzeuge und wie viele von Ihnen sich gerade auf dem Flughafen befinden
    public Dictionary<string, int> airplaneCapacities = new Dictionary<string, int>();
    public List<PlacedAsset> hangars = new List<PlacedAsset>();
    public List<PlacedAsset> terminals = new List<PlacedAsset>();
    //Dict aller Terminals, welche auf den besten Abstellplatz zeigen
    private Dictionary<PlacedAsset, List<Pathnode>> spaceTerminalPaths = new Dictionary<PlacedAsset, List<Pathnode>>();
    //Dict aller Hangars, welche auf den besten Abstellplatz zeigen
    public Dictionary<PlacedAsset, List<Pathnode>> spaceHangarPaths = new Dictionary<PlacedAsset, List<Pathnode>>();
    //Dict aller Abstellplätze, welche frei oder besetzt sind
    public Dictionary<PlacedAsset, bool> airplaneSpaces = new Dictionary<PlacedAsset, bool>();
    public HashSet<ActiveVehicle> readyToStartAirplanes = new HashSet<ActiveVehicle>();
    public static AirportManager Instance { get; private set; }

    [SerializeField] private float sendingInterval = 15f;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;

    }

    public void AddAirplanes(List<string> airplanes)
    {
        Debug.Log(airplanes);
        foreach (var airplane in airplanes)
        {
            airplaneCapacities[airplane] = airplaneCapacities.GetValueOrDefault(airplane, 0) + 1;
        }
    }

    public void AddAirplaneSpace(PlacedAsset asset)
    {
        airplaneSpaces[asset] = false;
        spaceTerminalPaths[asset] = new List<Pathnode>();
        spaceHangarPaths[asset] = new List<Pathnode>();
    }

    public ActiveVehicle PrepareAirplaneForTakeoff(string airplaneType)
    {
        if (airplaneCapacities.GetValueOrDefault(airplaneType, 0) == 0) return null;
        Vehicle airplane = VehicleManager.Instance.GetAirplane(airplaneType);
        if (airplane == null)
        {
            return null;
        }
        PlacedAsset parkingSpace = GetFreeSpace();
        if (parkingSpace == null) return null;
        return ActiveVehicle.Init(airplane, spaceHangarPaths[parkingSpace], true);
    }

    public void SendVehiclesToAirplane(ActiveVehicle activeVehicle, Vehicle vehicle, Vector3 position)
    {
        BuildingSystem
        if (IsTerminalSpace())
        {

            return;
        }
        int capacity = vehicle.capacity;
        int shuttles = capacity / VehicleManager.Instance.bus.capacity;
        int taxis = (int)Math.Ceiling((capacity % VehicleManager.Instance.bus.capacity) / (double)VehicleManager.Instance.taxi.capacity);
        StartCoroutine(SendPassangerTransportVehicles(shuttles, taxis, activeVehicle));
    }

    private IEnumerator SendPassangerTransportVehicles(int shuttles, int taxis, List<Pathnode> bestPath, ActiveVehicle activeVehicle)
    {
        while (shuttles > 0 || taxis > 0)
        {
            if (shuttles > 0)
            {
                shuttles--;
                ActiveVehicle.Init(VehicleManager.Instance.bus, bestPath, false);
            }
            else if (taxis > 0)
            {
                taxis--;
                ActiveVehicle.Init(VehicleManager.Instance.taxi, bestPath, false);
            }
            yield return sendingInterval;
        }
        readyToStartAirplanes.Add(activeVehicle);
        //checke im auto, ob es ziel erreicht hat, das auto hat eine rückfahrt flag
    }

    private bool IsTerminalSpace()
    {
        //get nachbarn
        //
    }

    public bool AirplaneExists(string airplaneType)
    {
        if (airplaneCapacities.GetValueOrDefault(airplaneType, 0) == 0) return false;
        return true;
    }

    public bool AirplaneReady(ActiveVehicle vehicle)
    {
        if(readyToStartAirplanes.Contains(vehicle)) {
            readyToStartAirplanes.Remove(vehicle);
            return true;
        }
        return false;
    }

    public PlacedAsset GetFreeSpace()
    {
        foreach (var kv in airplaneSpaces)
        {
            if (kv.Value) return kv.Key;
        }
        return null;
    }

    public void RecalculatePaths()
    {
        AddRoadNeighbours(hangars);
        AddRoadNeighbours(terminals);

        foreach (PlacedAsset airplaneSpace in airplaneSpaces.Keys)
        {
            //ermittle besten Path von allen Hangars zum AirplaneSpace
            List<Pathnode> bestHangar = GetBestPathToAirplaneSpace(hangars, airplaneSpace);
            spaceHangarPaths[airplaneSpace] = bestHangar;

            List<Pathnode> bestTerminal = GetBestPathToAirplaneSpace(terminals, airplaneSpace);
            spaceTerminalPaths[airplaneSpace] = bestTerminal;
        }
    }

    private List<Pathnode> GetBestPathToAirplaneSpace(ICollection<PlacedAsset> objects, PlacedAsset airplaneSpace)
    {
        //ermittle besten Path von allen Hangars zum AirplaneSpace
        List<Pathnode> bestList = new List<Pathnode>();
        Vector2Int end = airplaneSpace.origin;
        foreach (PlacedAsset obj in objects)
        {
            Vector2Int start = obj.origin;
            List<Pathnode> path = PathfindingManager.Instance.CalculatePath(start.x, start.y, end.x, end.y);
            if (path.Count < bestList.Count)
            {
                bestList = path;
            }
        }
        return bestList;
    }

    private void AddRoadNeighbours(ICollection<PlacedAsset> objects)
    {
        //bestimme nachbarn, welche roads sind
        foreach (PlacedAsset obj in objects)
        {
            List<PlacedAsset> assets = BuildingSystem.Instance.GetNeighbourAssets(obj);
            //ermittle ob Nachbarn Straßen sind, wenn ja, dann adde sie zum Pathnode vom jeweiligen Objekt
            foreach (var asset in assets)
            {
                if (asset.GetComponent<RoadAsset>() != null)
                {
                    BuildingSystem.Instance.AddNeighbourToGridObject(obj.origin, asset.origin, false);
                }
            }
        }
    }

    public void LoadData(Data data)
    {
        foreach (var airplaneCapactiy in data.airplaneCapacities)
        {
            airplaneCapacities.Add(airplaneCapactiy.vehicleName, airplaneCapactiy.storage);
        }
    }

    public void SaveData(Data data)
    {
        foreach (var airplaneCapactiy in airplaneCapacities)
        {
            data.airplaneCapacities.Add(new StorageSaveObject(airplaneCapactiy.Key, airplaneCapactiy.Value));
        }
    }
}
