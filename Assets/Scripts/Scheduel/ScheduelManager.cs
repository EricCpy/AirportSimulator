using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduelManager : MonoBehaviour, IData
{
    public DateTime airportTime { get; private set; }
    public static ScheduelManager Instance { get; private set; }
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
    }

    public void SaveData(Data data)
    {
        data.time = airportTime.ToString("o");
    }

    // Update is called once per frame

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
}
