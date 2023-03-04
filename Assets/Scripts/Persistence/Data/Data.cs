using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    //public Grid<GridObject> grid;
    public string hello;

    public List<AssetSaveObject> gridObjects;
    public Data()
    {
        hello = "hello";
        gridObjects = new List<AssetSaveObject>();
        //grid = new Grid<GridObject>(10, 10, 10f, Vector3.zero, (g, x, y) => new GridObject(g, x, y), true);
    }

    public void Clear() {
        gridObjects = new List<AssetSaveObject>();
    }
}
