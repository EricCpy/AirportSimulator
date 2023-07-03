using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Dictionary<PlacedAsset, (int, List<Pathnode>)> spaceRunwayPath = new Dictionary<PlacedAsset, (int, List<Pathnode>)>();
    //Dict aller Abstellplätze, welche frei oder besetzt sind
    public Dictionary<PlacedAsset, bool> airplaneSpaces = new Dictionary<PlacedAsset, bool>();
    private Dictionary<ActiveVehicle, PlacedAsset> spaceOfActiveAirplane = new Dictionary<ActiveVehicle, PlacedAsset>();
    public HashSet<ActiveVehicle> readyToStartAirplanes = new HashSet<ActiveVehicle>();
    public static AirportManager Instance { get; private set; }
    private Vector2Int lastSelectedRunwayStart = new Vector2Int(-1, -1), lastSelectedRunwayEnd = new Vector2Int(-1, -1);
    public List<List<Vector2Int>> runwayStartAndEnds { get; private set; } = new List<List<Vector2Int>>();
    public List<List<Pathnode>> runways { get; private set; } = new List<List<Pathnode>>();
    private List<Pathnode> arrivalPath = new List<Pathnode>();
    public int arrivalRunwayIndex = -1;
    private List<Barrier> driveOnBarriers = new List<Barrier>();
    private List<List<Barrier>> driveOffBarriers = new List<List<Barrier>>();
    [SerializeField] private float sendingInterval = 15f;
    [SerializeField] private Color runwayColor = new Color(1, 0.75f, 0, 1);
    [SerializeField] private GameObject barrierPrefab;
    private List<bool> airplaneOnRunwayList = new List<bool>();
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;
        PathfindingManager.Instance.OnInitialized += InitPaths;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || GameManager.Instance.uiOpen) return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            //anfang runway
            SetRunwayStart(BuildingSystem.Instance.MousePositionToGridPosition(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //ende runway
            SetRunwayEnd(BuildingSystem.Instance.MousePositionToGridPosition(Input.mousePosition));

        }
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DeleteRunways();
        }
    }

    private void InitPaths()
    {
        AirportManager.Instance.RecalculatePaths();
        foreach (var runwayStartAndEnd in runwayStartAndEnds)
        {
            lastSelectedRunwayStart = runwayStartAndEnd[0];
            lastSelectedRunwayEnd = runwayStartAndEnd[1];
            DetermineRunway(false);
        }
        DetermineArrivalPath();
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
        airplaneSpaces[parkingSpace] = true;
        ActiveVehicle activeVehicle = ActiveVehicle.Init(airplane, spaceHangarPaths[parkingSpace], spaceRunwayPath[parkingSpace].Item1);
        spaceOfActiveAirplane[activeVehicle] = parkingSpace;
        return activeVehicle;
    }

    public void PrepareRunwayForLanding(string airplaneType)
    {
        if (arrivalPath == null || arrivalPath.Count == 0) return;
        Vehicle airplane = VehicleManager.Instance.GetAirplane(airplaneType);
        driveOnBarriers[arrivalRunwayIndex].ToggleBlockStatus(false);
        if (airplane == null) return;
        airplaneCapacities[airplaneType]++;
        driveOnBarriers[arrivalRunwayIndex].ToggleBlockStatus(true);
        airplaneOnRunwayList[arrivalRunwayIndex] = true;
        foreach (var barrier in driveOffBarriers[arrivalRunwayIndex]) barrier.ToggleBlockStatus(false);
        ActiveVehicle activeVehicle = ActiveVehicle.Init(airplane, arrivalPath, arrivalRunwayIndex);
        activeVehicle.SetLastDrive(true);
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
        if (rest >= VehicleManager.Instance.bus.capacity / 2) shuttles++;
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
                shuttles--;
                ActiveVehicle.Init(VehicleManager.Instance.bus, bestPath);
            }
            else if (taxis > 0)
            {
                taxis--;
                ActiveVehicle.Init(VehicleManager.Instance.taxi, bestPath);
            }
            yield return delay;
        }
        readyToStartAirplanes.Add(activeVehicle);
    }

    private bool IsTerminalSpace(PlacedAsset airplaneSpace)
    {
        foreach (var asset in BuildingSystem.Instance.GetNeighbourAssets(airplaneSpace))
        {
            if (asset.GetGridAsset().assetName == "Terminal") return true;
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
            airplaneSpaces[spaceOfActiveAirplane[vehicle]] = false;
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
        PathfindingManager.Instance.ClearStats();
        AddRoadNeighbours(hangars);
        AddRoadNeighbours(terminals);
        AddRoadNeighbours(airplaneSpaces.Keys);
        //calculate spaces from parking
        List<Vector2Int> runwayStarts = new List<Vector2Int>();
        foreach (var runwayStartAndEnd in runwayStartAndEnds)
        {
            runwayStarts.Add(runwayStartAndEnd[0]);
        }
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //for (int i = 0; i < 10000; i++)
        //{
        foreach (PlacedAsset airplaneSpace in airplaneSpaces.Keys)
        {
            //ermittle besten Path von allen Hangars zum AirplaneSpace
            List<Pathnode> bestHangarPath = GetBestPathToPosition(hangars, airplaneSpace.origin);
            if (bestHangarPath.Count > 0) spaceHangarPaths[airplaneSpace] = bestHangarPath;
            List<Pathnode> bestTerminalPath = GetBestPathToPosition(terminals, airplaneSpace.origin);
            if (bestTerminalPath.Count > 0) spaceTerminalPaths[airplaneSpace] = bestTerminalPath;

            if (runwayStarts.Count > 0)
            {
                (int, List<Pathnode>) bestRunway = GetBestPathToRunway(runwayStarts, airplaneSpace.origin);
                bestRunway.Item2.Reverse();
                bestRunway.Item2.RemoveAt(bestRunway.Item2.Count - 1);
                spaceRunwayPath[airplaneSpace] = bestRunway;
            }
        }

        foreach (var path in spaceHangarPaths)
        {
            StringBuilder s = new StringBuilder("[");
            for (int i = 0; i < path.Value.Count; i++)
            {
                s.Append(path.Value[i] + ", ");
            }
            s.Append("]");
            UnityEngine.Debug.Log(s);
        }
        //}
        //stopwatch.Stop();
        //runTime = stopwatch.ElapsedMilliseconds;
        //Console.Write(runTime);
    }
    public double runTime = 0;
    private (int, List<Pathnode>) GetBestPathToRunway(List<Vector2Int> objects, Vector2Int end)
    {
        //ermittle besten Path von allen objects zum AirplaneSpace
        List<Pathnode> bestList = new List<Pathnode>();
        int bestIndex = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            Vector2Int start = objects[i];
            List<Pathnode> path = PathfindingManager.Instance.CalculatePath(start.x, start.y, end.x, end.y);
            if (path.Count > 0 && (bestList.Count == 0 || path.Count < bestList.Count))
            {
                bestList = path;
                bestIndex = i;
            }
        }
        return (bestIndex, bestList);
    }

    private List<Pathnode> GetBestPathToPosition(ICollection<PlacedAsset> objects, Vector2Int end)
    {
        //ermittle besten Path von allen objects zum AirplaneSpace
        List<Pathnode> bestList = new List<Pathnode>();
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
            //lösche alle nachbarn von der obj origin
            BuildingSystem.Instance.DeleteNeighboursFromPlacedAsset(obj.origin);
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

    public PlacedAsset GetActiveAirplaneSpace(ActiveVehicle activeAirplane)
    {
        return spaceOfActiveAirplane[activeAirplane];
    }


    public List<Pathnode> GetSpaceToRunwayPath(PlacedAsset space, int runwayIndex)
    {
        if (!spaceRunwayPath.ContainsKey(space) || runways.Count == 0) return null;
        List<Pathnode> way = new List<Pathnode>(spaceRunwayPath[space].Item2);
        way.AddRange(runways[runwayIndex]);
        return way;
    }

    private void DetermineRunway(bool addToStartAndEnd)
    {
        Vector2Int bound = new Vector2Int(-1, -1);
        if (lastSelectedRunwayStart == bound || lastSelectedRunwayEnd == bound) return;
        List<Pathnode> runway = PathfindingManager.Instance.CalculatePath(lastSelectedRunwayStart.x, lastSelectedRunwayStart.y, lastSelectedRunwayEnd.x, lastSelectedRunwayEnd.y);
        if (runway == null && runway.Count > 0) return;
        runways.Add(runway);
        airplaneOnRunwayList.Add(false);
        int runwayIndex = runways.Count - 1;
        for (int i = 0; i < runway.Count; i++)
        {
            Pathnode node = runway[i];
            GridObject gridObject = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y);
            PlacedAsset asset = gridObject.GetPlacedObject();
            SpriteRenderer sr = asset.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = runwayColor;
            foreach (var neighbour in gridObject.GetNeighbours())
            {
                if ((i > 0 && neighbour.node == runway[i - 1]) || (i < runway.Count - 1 && neighbour.node == runway[i + 1])) continue;
                if (neighbour.GetPlacedObject().GetComponent<RoadAsset>() != null)
                {
                    Pathnode neighbourNode = neighbour.node;
                    Vector3 pos = neighbourNode.origin;
                    float rot = 0;
                    if (neighbourNode.x != node.x)
                    {
                        if (neighbourNode.x < node.x) pos.x += BuildingSystem.Instance.GetCellSize();
                        rot = 90;
                    }
                    if (neighbourNode.y < node.y)
                    {
                        pos.y += BuildingSystem.Instance.GetCellSize();
                    }
                    Barrier barrier = Instantiate(barrierPrefab, pos, Quaternion.Euler(0, 0, rot)).GetComponent<Barrier>();
                    barrier.runwayIndex = runwayIndex;
                    if (driveOnBarriers.Count < runways.Count)
                    {
                        barrier.runwayDriveOn = true;
                        driveOnBarriers.Add(barrier);
                        driveOffBarriers.Add(new List<Barrier>());
                    }
                    else driveOffBarriers[runwayIndex].Add(barrier);
                }
            }
        }
        if (addToStartAndEnd)
        {
            List<Vector2Int> runwayStartAndEnd = new List<Vector2Int>();
            runwayStartAndEnd.Add(lastSelectedRunwayStart);
            runwayStartAndEnd.Add(lastSelectedRunwayEnd);
            runwayStartAndEnds.Add(runwayStartAndEnd);
        }
        lastSelectedRunwayStart = new Vector2Int(-1, -1);
        lastSelectedRunwayEnd = new Vector2Int(-1, -1);
    }

    private void DetermineArrivalPath()
    {
        if (runways.Count == 0 || hangars.Count == 0) return;
        for (int i = 0; i < runwayStartAndEnds.Count; i++)
        {
            List<Pathnode> hangarPath = GetBestPathToPosition(hangars, runwayStartAndEnds[i][1]);
            if (arrivalPath.Count == 0 || hangarPath.Count < arrivalPath.Count)
            {
                arrivalPath = hangarPath;
                arrivalRunwayIndex = i;
            }
        }
        arrivalPath.Reverse();
    }

    public void SetRunwayStart(Vector2Int xy)
    {
        lastSelectedRunwayStart = xy;
        DetermineRunway(true);
    }

    public void SetRunwayEnd(Vector2Int xy)
    {
        lastSelectedRunwayEnd = xy;
        DetermineRunway(true);
        DetermineArrivalPath();
    }

    public void DeleteRunways()
    {
        if (runways == null) return;
        UnityEngine.Debug.Log("1");
        runwayStartAndEnds.Clear();
        for (int i = 0; i < driveOnBarriers.Count; i++)
        {
            Destroy(driveOnBarriers[i].gameObject);
        }
        driveOnBarriers.Clear();
        for (int i = 0; i < driveOffBarriers.Count; i++)
        {
            foreach (var barrier in driveOffBarriers[i]) Destroy(barrier.gameObject);
        }
        driveOffBarriers.Clear();
        for (int i = 0; i < runways.Count; i++)
        {
            UnityEngine.Debug.Log("unrway " + i);
            foreach (Pathnode node in runways[i])
            {
                var placedAsset = BuildingSystem.Instance.grid.GetValue(node.gridPosition.x, node.gridPosition.y).GetPlacedObject();
                var sr = placedAsset.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = Color.white;
            }
        }
        UnityEngine.Debug.Log("2");
        runways.Clear(); ;
    }

    public void LoadData(Data data)
    {
        foreach (var airplaneCapactiy in data.airplaneCapacities)
        {
            airplaneCapacities.Add(airplaneCapactiy.vehicleName, airplaneCapactiy.storage);
        }
        foreach (var startAndEnd in data.runwayStartAndEnds)
        {
            runwayStartAndEnds.Add(new List<Vector2Int>(startAndEnd.list));
        }
    }

    public void SaveData(Data data)
    {
        foreach (var airplaneCapactiy in airplaneCapacities)
        {
            data.airplaneCapacities.Add(new StorageSaveObject(airplaneCapactiy.Key, airplaneCapactiy.Value));
        }
        foreach (var startAndEnd in runwayStartAndEnds)
        {
            var nl = new NestedList();
            nl.list = new List<Vector2Int>(startAndEnd);
            data.runwayStartAndEnds.Add(nl);
        }
    }

    public void BlockRunway(int runwayIndex)
    {
        if (driveOnBarriers[runwayIndex].IsBlocked()) return;
        driveOnBarriers[runwayIndex].ToggleBlockStatus(true);
        foreach (var barrier in driveOffBarriers[runwayIndex])
        {
            barrier.ToggleBlockStatus(true);
        }
        airplaneOnRunwayList[runwayIndex] = false;
    }

    public void AirplaneLeftOrEnteredRunway(bool enteredRunway, int runwayIndex)
    {
        if (enteredRunway)
        {
            UnityEngine.Debug.Log("ff32");
            BlockRunway(runwayIndex);
            airplaneOnRunwayList[runwayIndex] = true;
        }
        else if (airplaneOnRunwayList[runwayIndex])
        {
            driveOnBarriers[runwayIndex].ToggleBlockStatus(false);
            foreach (var barrier in driveOffBarriers[runwayIndex])
            {
                barrier.ToggleBlockStatus(false);
            }
            airplaneOnRunwayList[runwayIndex] = false;
        }
    }

    public bool IsAirplaneOnRunway(int runwayIndex)
    {
        if (runwayIndex < 0 || runwayIndex >= airplaneOnRunwayList.Count) return false;
        return airplaneOnRunwayList[runwayIndex];
    }
}
