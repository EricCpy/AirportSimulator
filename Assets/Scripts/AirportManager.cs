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
    private Vector2Int runwayStart = new Vector2Int(-1, -1), runwayEnd = new Vector2Int(-1, -1);
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
        PathfindingManager.Instance.OnInitialized += InitPaths;
    }

    private void InitPaths()
    {
        AirportManager.Instance.SetRunwayStart(runwayStart);
        AirportManager.Instance.SetRunwayEnd(runwayEnd);
        AirportManager.Instance.RecalculatePaths();
        //foreach(var hangar in hangars) Debug.Log(BuildingSystem.Instance.grid.GetValue(hangar.origin.x, hangar.origin.y).neighbours.Count);
    }

    public void AddAirplanes(List<string> airplanes)
    {
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
        if (airplane == null) return null;
        airplaneCapacities[airplaneType]--;
        PlacedAsset parkingSpace = GetFreeSpace();
        if (parkingSpace == null) return null;
        ActiveVehicle activeVehicle = ActiveVehicle.Init(airplane, spaceHangarPaths[parkingSpace], true);
        Debug.Log(activeVehicle);
        spaceOfActiveAirplane[activeVehicle] = parkingSpace;
        return activeVehicle;
    }

    public void SendVehiclesToAirplane(ActiveVehicle activeVehicle, Vehicle vehicle, Vector3 position)
    {
        PlacedAsset airplaneSpace = BuildingSystem.Instance.grid.GetValue(position).GetPlacedObject();
        if (IsTerminalSpace(airplaneSpace))
        {
            readyToStartAirplanes.Add(activeVehicle);
            return;
        }
        int capacity = vehicle.capacity;
        int shuttles = capacity / VehicleManager.Instance.bus.capacity;
        int rest = (capacity % VehicleManager.Instance.bus.capacity);
        int taxis = 0;
        if(rest >= VehicleManager.Instance.bus.capacity / 2) shuttles++;
        else taxis = (int)Math.Ceiling(rest / (double)VehicleManager.Instance.taxi.capacity);
        StartCoroutine(SendPassangerTransportVehicles(shuttles, taxis, spaceTerminalPaths[airplaneSpace], activeVehicle));
    }

    private IEnumerator SendPassangerTransportVehicles(int shuttles, int taxis, List<Pathnode> bestPath, ActiveVehicle activeVehicle)
    {
        var delay = new WaitForSeconds(sendingInterval);
        while (shuttles > 0 || taxis > 0)
        {
            if (shuttles > 0)
            {
                Debug.Log("shuttles");
                shuttles--;
                ActiveVehicle.Init(VehicleManager.Instance.bus, bestPath, false);
            }
            else if (taxis > 0)
            {
                Debug.Log("taxis");
                taxis--;
                ActiveVehicle.Init(VehicleManager.Instance.taxi, bestPath, false);
            }
            yield return delay;
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
            if (!kv.Value) return kv.Key;
        }
        return null;
    }

    public void RecalculatePaths()
    {
        AddRoadNeighbours(hangars);
        AddRoadNeighbours(terminals);
        AddRoadNeighbours(airplaneSpaces.Keys);

        foreach (PlacedAsset airplaneSpace in airplaneSpaces.Keys)
        {
            //ermittle besten Path von allen Hangars zum AirplaneSpace
            List<Pathnode> bestHangarPath = GetBestPathToAirplaneSpace(hangars, airplaneSpace);
            if(bestHangarPath.Count > 0) spaceHangarPaths[airplaneSpace] = bestHangarPath;

            List<Pathnode> bestTerminalPath = GetBestPathToAirplaneSpace(terminals, airplaneSpace);
            if(bestTerminalPath.Count > 0) spaceTerminalPaths[airplaneSpace] = bestTerminalPath;

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
            if (path.Count > 0 && (bestList.Count == 0 || path.Count < bestList.Count))
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
                    BuildingSystem.Instance.AddNeighbourToGridObject(obj.origin, asset.origin);
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
        if (runway == null && runway.Count > 0) return;
        if (this.runway != null && this.runway.Count > 0)
        {
            foreach (Pathnode node in this.runway)
            {
                var sr = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y).GetPlacedObject().GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = Color.white;
            }
        }

        this.runway = runway;
        foreach (Pathnode node in runway)
        {
            var sr = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y).GetPlacedObject().GetComponentInChildren<SpriteRenderer>();
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
        runwayStart = data.runwayStart;
        runwayEnd = data.runwayEnd;
    }

    public void SaveData(Data data)
    {
        foreach (var airplaneCapactiy in airplaneCapacities)
        {
            data.airplaneCapacities.Add(new StorageSaveObject(airplaneCapactiy.Key, airplaneCapactiy.Value));
        }
        data.runwayEnd = runwayEnd;
        data.runwayStart = runwayStart;
        Debug.Log(data.runwayEnd);
        Debug.Log(data.runwayStart);
    }
}
