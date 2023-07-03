using UnityEngine;

public class AirplaneParkingSpace : MonoBehaviour
{
    PlacedAsset gridAsset;
    private void Awake()
    {
        gridAsset = GetComponent<PlacedAsset>();
        AirportManager.Instance.AddAirplaneSpace(gridAsset);
    }

    private void OnDestroy() {
        AirportManager.Instance.airplaneSpaces.Remove(gridAsset);
    }
}
