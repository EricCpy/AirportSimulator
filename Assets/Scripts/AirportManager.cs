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
    //Dict mit Liste mit generell besten Weg zum Runway
    public Dictionary<PlacedAsset, List<Pathnode>> spaceRunwayPath = new Dictionary<PlacedAsset, List<Pathnode>>();
    //Dict aller Abstellplätze, welche frei oder besetzt sind
    public Dictionary<PlacedAsset, bool> airplaneSpaces = new Dictionary<PlacedAsset, bool>();
    private Dictionary<ActiveVehicle, PlacedAsset> spaceOfActiveAirplane = new Dictionary<ActiveVehicle, PlacedAsset> ();
    public HashSet<ActiveVehicle> readyToStartAirplanes = new HashSet<ActiveVehicle>();
    public static AirportManager Instance { get; private set; }
    private Vector2Int runwayStart, runwayEnd;
    public List<Pathnode> runway {get; private set;}
    [SerializeField] private float sendingInterval = 15f;
    [SerializeField] private Color runwayColor = new Color(1, 0.75f, 0, 1);
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
        var activeVehicle = ActiveVehicle.Init(airplane, spaceHangarPaths[parkingSpace], true);
        spaceOfActiveAirplane[activeVehicle] = parkingSpace;
        return activeVehicle;
    }

    public void SendVehiclesToAirplane(ActiveVehicle activeVehicle, Vehicle vehicle, Vector3 position)
    {
        PlacedAsset airplaneSpace = BuildingSystem.Instance.grid.GetValue(position).asset;
        if (IsTerminalSpace(airplaneSpace))
        {
            readyToStartAirplanes.Add(activeVehicle);
            return;
        }
        int capacity = vehicle.capacity;
        int shuttles = capacity / VehicleManager.Instance.bus.capacity;
        int taxis = (int)Math.Ceiling((capacity % VehicleManager.Instance.bus.capacity) / (double)VehicleManager.Instance.taxi.capacity);
        StartCoroutine(SendPassangerTransportVehicles(shuttles, taxis, spaceTerminalPaths[airplaneSpace], activeVehicle));
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
    }

    private bool IsTerminalSpace(PlacedAsset airplaneSpace)
    {
        foreach (var asset in BuildingSystem.Instance.GetNeighbourAssets(airplaneSpace))
        {
            if (asset.GetGridAsset().assetName == "Planestop") return true;
        }
        return false;
    }

    public bool AirplaneExists(string airplaneType)
    {
        if (airplaneCapacities.GetValueOrDefault(airplaneType, 0) == 0) return false;
        return true;
    }

    public bool AirplaneReady(ActiveVehicle vehicle)
    {
        if (readyToStartAirplanes.Contains(vehicle))
        {
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

            if(runway != null) {
                List<Pathnode> bestRunwayPath = PathfindingManager.Instance.CalculatePath(airplaneSpace.origin.x, airplaneSpace.origin.y, runwayStart.x, runwayStart.y);
                spaceRunwayPath[airplaneSpace] = bestRunwayPath;
            }
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

    public PlacedAsset GetActiveAirplaneSpace(ActiveVehicle activeAirplane) {
        return spaceOfActiveAirplane[activeAirplane];
    }


    public List<Pathnode> GetSpaceToRunwayPath(PlacedAsset space) {
        return spaceRunwayPath[space];
    }

    private void DetermineRunway()
    {
        Vector2Int bound = new Vector2Int(-1, -1);
        if (runwayStart == bound || runwayEnd == bound) return;
        List<Pathnode> runway = PathfindingManager.Instance.CalculatePath(runwayStart.x, runwayStart.y, runwayEnd.x, runwayEnd.y);
        if (runway == null) return;
        if (this.runway != null)
        {
            foreach (Pathnode node in runway)
            {
                var sr = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y).asset.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = Color.white;
            }
        }
        this.runway = runway;
        foreach (Pathnode node in runway)
        {
            var sr = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y).asset.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = runwayColor;
        }
    }

    public void SetRunwayStart(Vector2Int xy)
    {
        runwayStart = xy;
        DetermineRunway();
    }

    public void SetRunwayEnd(Vector2Int xy)
    {
        runwayEnd = xy;
        DetermineRunway();
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
        data.runwayEnd = runwayEnd;
        data.runwayStart = runwayStart;
    }
}
