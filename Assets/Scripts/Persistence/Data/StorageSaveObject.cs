
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
