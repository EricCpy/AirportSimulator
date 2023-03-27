using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneParkingSpace : MonoBehaviour
{
    PlacedAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>();
        AirportManager.Instance.airplaneSpaces.Add(gridAsset, false);
    }

    private void OnDestroy() {
        AirportManager.Instance.airplaneSpaces.Remove(gridAsset);
    }
}
