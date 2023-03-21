using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduelManager : MonoBehaviour, IData
{
    public DateTime airportTime { get; private set; }
    public static ScheduelManager Instance { get; private set; }
    private SortedList<DateTime, ScheduelObject> scheduel = new SortedList<DateTime, ScheduelObject>();
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("ScheduelManager has already an Instance");
        }
        Instance = this;
        StartCoroutine(UpdateAirportTime());
    }

    public void LoadData(Data data)
    {
        airportTime = DateTime.Parse(data.time);
        foreach (var obj in data.scheduelObjects)
        {
            DateTime time = DateTime.Parse(obj.time);
            scheduel.Add(time, new ScheduelObject(time, obj.vehicleType, obj.flightType));
        }
    }

    public void SaveData(Data data)
    {
        data.time = airportTime.ToString("o");
        foreach (var obj in scheduel.Values)
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
            Debug.Log(airportTime);
            yield return delay;
        }
    }

    public ScheduelObject CreateNewScheduelEntry(DateTime time, Vehicle.VehicleType vehicleType, ScheduelObject.FlightType flightType)
    {
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
            scheduel.Remove(obj.time);
        }
    }
}
