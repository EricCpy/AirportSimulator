using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScheduelManager : MonoBehaviour, IData
{
    public DateTime airportTime { get; private set; }
    public static ScheduelManager Instance { get; private set; }
    private SortedList<DateTime, ScheduelObject> takeOffScheduel = new SortedList<DateTime, ScheduelObject>();
    private SortedList<DateTime, ScheduelObject> landingScheduel = new SortedList<DateTime, ScheduelObject>();
    private SortedList<DateTime, ActiveVehicle> activeAirplanes = new SortedList<DateTime, ActiveVehicle>();
    [SerializeField] private int checkingSeconds = 10;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("ScheduelManager has already an Instance");
        }
        Instance = this;
        StartCoroutine(UpdateAirportTime());
        StartCoroutine(PrepareFlights(checkingSeconds));
        StartCoroutine(StartFlights(checkingSeconds));
        StartCoroutine(CheckForLandings(checkingSeconds));
    }

    public void LoadData(Data data)
    {
        airportTime = DateTime.Parse(data.time);
        foreach (var obj in data.scheduelObjects)
        {
            DateTime time = DateTime.Parse(obj.time);
            if (obj.flightType == ScheduelObject.FlightType.Takeoff)
            {
                takeOffScheduel.Add(time, new ScheduelObject(time, obj.vehicleType, obj.flightType));
            }
            else
            {
                landingScheduel.Add(time, new ScheduelObject(time, obj.vehicleType, obj.flightType));
            }

        }
    }

    public void SaveData(Data data)
    {
        data.time = airportTime.ToString("o");
        foreach (var obj in takeOffScheduel.Values)
        {
            data.scheduelObjects.Add(obj.ToScheduelSaveObject());
        }
        foreach (var obj in landingScheduel.Values)
        {
            data.scheduelObjects.Add(obj.ToScheduelSaveObject());
        }
    }

    private IEnumerator UpdateAirportTime()
    {
        var delay = new WaitForSeconds(1);
        while (true)
        {
            airportTime = airportTime.AddSeconds(1);
            yield return delay;
        }
    }

    public ScheduelObject CreateNewScheduelEntry(DateTime time, string vehicleType, ScheduelObject.FlightType flightType)
    {
        SortedList<DateTime, ScheduelObject> scheduel = flightType == ScheduelObject.FlightType.Takeoff ? takeOffScheduel : landingScheduel;
        while (scheduel.ContainsKey(time))
        {
            time = time.AddMilliseconds(1);
        }
        ScheduelObject scheduelObj = new ScheduelObject(time, vehicleType, flightType);
        scheduel.Add(time, scheduelObj);
        return scheduelObj;
    }

    public void RemoveScheduelEntrys(ICollection<ScheduelObject> scheduelObjects)
    {
        foreach (var obj in scheduelObjects)
        {
            if (obj.flightType == ScheduelObject.FlightType.Takeoff)
            {
                takeOffScheduel.Remove(obj.time);
            }
            else
            {
                landingScheduel.Remove(obj.time);
            }

        }
    }

    public ICollection<ScheduelObject> GetAllScheduelEntries()
    {
        return takeOffScheduel.Values.Concat(landingScheduel.Values).ToList();
    }

    private IEnumerator PrepareFlights(int time)
    {
        var delay = new WaitForSeconds(time);
        while (true)
        {
            if (takeOffScheduel.Count > 0 && (takeOffScheduel.First().Key - airportTime) <= TimeSpan.FromMinutes(30) &&
                AirportManager.Instance.AirplaneExists(takeOffScheduel.First().Value.vehicleType))
            {
                ActiveVehicle plane = AirportManager.Instance.PrepareAirplaneForTakeoff(takeOffScheduel.First().Value.vehicleType);
                var kvpair = takeOffScheduel.First();
                takeOffScheduel.Remove(kvpair.Key);
                if (plane == null)
                {
                    ScheduelObject val = kvpair.Value;
                    DateTime newTime = val.time.AddMinutes(10);
                    CreateNewScheduelEntry(newTime, val.vehicleType, val.flightType);
                }
                else
                {
                    activeAirplanes.Add(kvpair.Key, plane);
                }
            }
            yield return delay;
        }
    }

    private IEnumerator StartFlights(int time)
    {
        var delay = new WaitForSeconds(time);
        while (true)
        {
            if (activeAirplanes.Count > 0 && activeAirplanes.First().Key <= airportTime && (landingScheduel.Count == 0 || (landingScheduel.First().Key - airportTime) <= TimeSpan.FromMinutes(5)))
            {
                var kvpair = activeAirplanes.First();
                activeAirplanes.Remove(kvpair.Key);
                if (AirportManager.Instance.AirplaneReady(kvpair.Value))
                {
                    var airplaneSpace = AirportManager.Instance.GetActiveAirplaneSpace(kvpair.Value);
                    kvpair.Value.InitPath(AirportManager.Instance.GetSpaceToRunwayPath(airplaneSpace));
                    kvpair.Value.SetRunway(true);
                }
                else
                {
                    DateTime dateTime = kvpair.Key.AddMinutes(10);
                    activeAirplanes.Add(dateTime, kvpair.Value);
                }
            }
            yield return delay;
        }
    }

    private IEnumerator CheckForLandings(int time)
    {
        var delay = new WaitForSeconds(time);
        while (true)
        {
            if (landingScheduel.Count > 0 && landingScheduel.First().Key <= airportTime)
            {
                var kvpair = landingScheduel.First();
                landingScheduel.Remove(kvpair.Key);
                //create Airplane welche zu hangar fährt
                AirportManager.Instance.PrepareRunwayForLanding(kvpair.Value.vehicleType);
            }
            yield return delay;
        }
    }

}
