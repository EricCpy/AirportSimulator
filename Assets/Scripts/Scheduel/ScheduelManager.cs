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
    [SerializeField] private int checkingSeconds = 10;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("ScheduelManager has already an Instance");
        }
        Instance = this;
        StartCoroutine(UpdateAirportTime());
        StartCoroutine(CheckFlights(checkingSeconds));
    }

    public void LoadData(Data data)
    {
        airportTime = DateTime.Parse(data.time);
        foreach (var obj in data.scheduelObjects)
        {
            DateTime time = DateTime.Parse(obj.time);
            if(obj.flightType == ScheduelObject.FlightType.Takeoff) {
                takeOffScheduel.Add(time, new ScheduelObject(time, obj.vehicleType, obj.flightType));
            } else {
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
        var delay = new WaitForSecondsRealtime(1);
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
            if(obj.flightType == ScheduelObject.FlightType.Takeoff) {
                takeOffScheduel.Remove(obj.time);
            } else {
                landingScheduel.Remove(obj.time);
            }
            
        }
    }

    public ICollection<ScheduelObject> GetAllScheduelEntries() {
        return takeOffScheduel.Values.Concat(landingScheduel.Values).ToList();
    }

    private IEnumerator CheckFlights(int time)
    {
        var delay = new WaitForSecondsRealtime(time);
        while (true)
        {
            //TODO checke
            //checke ob erste airplane in 30min los muss, dann checke ob es preparebar ist
                //wenn preparebar, dann mache nichts
                //wenn nicht, dann adde 10min auf den schedueleintrag 
            //if()
            yield return delay;
        }
    }
}
