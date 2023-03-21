using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int height, width;
    public List<AssetSaveObject> gridObjects;
    public List<VehicleSaveObject> vehicleObjects;
    public List<ScheduelSaveObject> scheduelObjects;
    public string time;
    public Data(int width, int height, DateTime time)
    {
        this.height = height;
        this.width = width;
        this.time = time.ToString("o");
        this.gridObjects = new List<AssetSaveObject>();
        this.vehicleObjects = new List<VehicleSaveObject>();
        this.scheduelObjects = new List<ScheduelSaveObject>();
    }

    public void Clear()
    {
        gridObjects = new List<AssetSaveObject>();
        vehicleObjects = new List<VehicleSaveObject>();
        scheduelObjects = new List<ScheduelSaveObject>();
    }
}
