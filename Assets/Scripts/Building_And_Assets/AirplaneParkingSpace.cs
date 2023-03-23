using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneParkingSpace : MonoBehaviour
{
    GridAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>().GetGridAsset();
        AirportManager.Instance.airplaneSpace.Add(gridAsset, false);
    }

    private void OnDestroy() {
        AirportManager.Instance.airplaneSpace.Remove(gridAsset);
    }
}
