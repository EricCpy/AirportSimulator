using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hangar : MonoBehaviour
{
    PlacedAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>();
        AirportManager.Instance.hangars.Add(gridAsset);
        Debug.Log(BuildingSystem.Instance.assetsLoaded);
        if (BuildingSystem.Instance.assetsLoaded)
        {
            InGameUI.Instance.OpenHangarCreationUI();
        }
    }

    private void OnDestroy()
    {
        AirportManager.Instance.hangars.Remove(gridAsset);
    }
}
