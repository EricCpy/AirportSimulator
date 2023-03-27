using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportManager : MonoBehaviour, IData
{
    //Dict aller Flugzeuge und wie viele von Ihnen sich gerade auf dem Flughafen befinden
    public Dictionary<string, int> airplaneCapacities = new Dictionary<string, int>();
    //Dict aller Terminals, welche auf den besten Abstellplatz zeigen
    public Dictionary<PlacedAsset, Dictionary<PlacedAsset, List<Pathnode>>> terminals = new Dictionary<PlacedAsset, Dictionary<PlacedAsset, List<Pathnode>>>();
    //Dict aller Hangars, welche auf den besten Abstellplatz zeigen
    public Dictionary<PlacedAsset, Dictionary<PlacedAsset, List<Pathnode>>> hangars = new Dictionary<PlacedAsset, Dictionary<PlacedAsset, List<Pathnode>>>();
    //Dict aller Abstellplätze, welche frei oder besetzt sind
    public Dictionary<PlacedAsset, bool> airplaneSpaces = new Dictionary<PlacedAsset, bool>();
    public static AirportManager Instance { get; private set; }
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

    public bool PrepareAirplaneForTakeoff(string airplaneType)
    {
        if (airplaneCapacities.GetValueOrDefault(airplaneType, 0) == 0) return false;

        //TODO
        //sende Flugzeug zum abstellPlatz
        //StarteCoroutine um Taxis etc zum Landeplatz zu senden
        return true;
    }

    public void RecalculatePaths()
    {
        AddRoadNeighbours(hangars.Keys);
        AddRoadNeighbours(terminals.Keys);

        foreach (PlacedAsset airplaneSpace in airplaneSpaces.Keys)
        {
            //ermittle besten Path von allen Hangars zum AirplaneSpace
            (PlacedAsset, List<Pathnode>) bestHangar = GetBestPathToAirplaneSpace(hangars.Keys, airplaneSpace);
            if(bestHangar.Item1 != null) hangars[bestHangar.Item1].Add(airplaneSpace, bestHangar.Item2);

            (PlacedAsset, List<Pathnode>) bestTerminal = GetBestPathToAirplaneSpace(terminals.Keys, airplaneSpace);
            if(bestTerminal.Item1 != null) terminals[bestTerminal.Item1].Add(airplaneSpace, bestTerminal.Item2);
        }
    }

    private (PlacedAsset, List<Pathnode>) GetBestPathToAirplaneSpace(ICollection<PlacedAsset> objects, PlacedAsset airplaneSpace)
    {
        //ermittle besten Path von allen Hangars zum AirplaneSpace
        PlacedAsset best = null;
        List<Pathnode> bestList = new List<Pathnode>();
        Vector2Int end = airplaneSpace.origin;
        foreach (PlacedAsset obj in objects)
        {
            Vector2Int start = obj.origin;
            List<Pathnode> path = PathfindingManager.Instance.CalculatePath(start.x, start.y, end.x, end.y);
            if(path.Count < bestList.Count) {
                best = obj;
                bestList = path;
            }
        }
        return (best, bestList); 
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
