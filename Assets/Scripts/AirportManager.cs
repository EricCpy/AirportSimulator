using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportManager : MonoBehaviour
{
    //Dict aller Flugzeuge und wie viele von Ihnen sich gerade auf dem Flughafen befinden
    public Dictionary<string, int> airplaneCapacities = new Dictionary<string, int>();
    //Dict aller Terminals, welche auf den besten Abstellplatz zeigen
    public Dictionary<GridAsset, List<GridAsset>> terminals = new Dictionary<GridAsset, List<GridAsset>>();
    //Dict aller Hangars, welche auf den besten Abstellplatz zeigen
    public Dictionary<GridAsset, List<GridAsset>> hangars = new Dictionary<GridAsset, List<GridAsset>>();
    //Dict aller Abstellplätze, welche frei oder besetzt sind
    public Dictionary<GridAsset, bool> airplaneSpace = new Dictionary<GridAsset, bool>();
    public static AirportManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;

    }
    
}
