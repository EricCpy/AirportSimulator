
[System.Serializable]
public class ScheduelSaveObject
{
    public string time;
    public string vehicleType;
    public ScheduelObject.FlightType flightType;
    public ScheduelSaveObject(string time, string vehicleType, ScheduelObject.FlightType flightType)
    {
        this.time = time;
        this.vehicleType = vehicleType;
        this.flightType = flightType;
    }
}
