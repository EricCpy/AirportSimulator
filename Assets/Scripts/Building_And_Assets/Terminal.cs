using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    GridAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>().GetGridAsset();
        AirportManager.Instance.terminals.Add(gridAsset, new List<GridAsset>());
    }

    private void OnDestroy() {
        AirportManager.Instance.terminals.Remove(gridAsset);
    }
}
