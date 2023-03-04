using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int height, width;

    public List<AssetSaveObject> gridObjects;
    public Data(int width, int height)
    {
        height = 0;
        width = 0;
        gridObjects = new List<AssetSaveObject>();
    }

    public void Clear()
    {
        gridObjects = new List<AssetSaveObject>();
    }
}
