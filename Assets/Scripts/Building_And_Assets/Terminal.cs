using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    PlacedAsset gridAsset;
    private void Start()
    {
        gridAsset = GetComponent<PlacedAsset>();
        AirportManager.Instance.terminals.Add(gridAsset, new List<GridAsset>());
    }

    private void OnDestroy()
    {
        AirportManager.Instance.terminals.Remove(gridAsset);
    }
}
