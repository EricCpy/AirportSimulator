using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StorageSaveObject
{
    public string vehicleName;
    public int storage;

    public StorageSaveObject(string vehicleName, int storage)
    {
        this.vehicleName = vehicleName;
        this.storage = storage;
    }
}
