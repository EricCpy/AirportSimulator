using UnityEngine;

public class Hangar : MonoBehaviour
{
    PlacedAsset gridAsset;
    private void Awake()
    {
        gridAsset = GetComponent<PlacedAsset>();
        AirportManager.Instance.hangars.Add(gridAsset);
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
