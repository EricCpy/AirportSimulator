using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int height, width;

    public List<AssetSaveObject> gridObjects;
    public Data()
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
