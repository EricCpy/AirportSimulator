using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hangar : MonoBehaviour
{
    GridAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>().GetGridAsset();
        AirportManager.Instance.hangars.Add(gridAsset, new List<GridAsset>());
        if(BuildingSystem.Instance.assetsLoaded) {
            //TODO
            //openUI zum Auswählen der Flugzeugtypen
        }
    }

    private void OnDestroy() {
        AirportManager.Instance.hangars.Remove(gridAsset);
    }
}
