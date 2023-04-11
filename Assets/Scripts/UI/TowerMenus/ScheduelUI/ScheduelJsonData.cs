

using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
[System.Serializable]
public class ScheduelJsonData
{
    /*data like {
        "arrivals":[
           {
              "arrivalTime":"11-04-2023 21:52:33",
              "airplaneType":"A"
           }
        ],
        "departures":[
           {
              "departureTime":"11-04-2023 21:52:40",
              "airplaneType":"A"
           }
        ]
         }*/
    public List<Arrival> arrivals;
    public List<Departure> departures;

    public ScheduelJsonData(List<Arrival> arrivals, List<Departure> departures)
    {
        this.arrivals = arrivals;
        this.departures = departures;
    }


    [System.Serializable]
    public class Arrival
    {
        public string arrivalTime;
        public string airplaneType;

        public override string ToString()
        {
            return "ArrivalTime: " + arrivalTime + ", AirplaneType: " + airplaneType;
        }
    }

    [System.Serializable]
    public class Departure
    {
        public string departureTime;
        public string airplaneType;

        public override string ToString()
        {
            return "DepartureTime: " + departureTime + ", AirplaneType: " + airplaneType;
        }
    }

}