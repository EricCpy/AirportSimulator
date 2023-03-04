using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int height, width;

    public List<AssetSaveObject> gridObjects;
    public Data(int width, int height)
    {
        this.height = height;
        this.width = width;
        gridObjects = new List<AssetSaveObject>();
    }

    public void Clear()
    {
        gridObjects = new List<AssetSaveObject>();
    }
}
