using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//platziertes Asset
public class PlacedAsset : MonoBehaviour, IData
{
    public static PlacedAsset Init(Vector3 worldPos, Vector2Int origin, GridAsset.AssetRotation rot, GridAsset gridAsset)
    {
        Transform assetTransform = Instantiate(gridAsset.prefab, worldPos, Quaternion.Euler(0, gridAsset.GetRotationAngle(rot), 0));
        PlacedAsset asset = assetTransform.GetComponent<PlacedAsset>();
        asset.Create(gridAsset, origin, rot);
        return asset;
    }

    private GridAsset gridAsset;
    private Vector2Int origin;
    private GridAsset.AssetRotation rot;

    private void Create(GridAsset gridAsset, Vector2Int origin, GridAsset.AssetRotation rot)
    {
        this.gridAsset = gridAsset;
        this.origin = origin;
        this.rot = rot;
    }

    public List<Vector2Int> GetPositions()
    {
        return gridAsset.GetPositions(origin, rot);
    }

    public void DestroyAsset()
    {
        Destroy(gameObject);
    }

    public void LoadData(Data data)
    {
        throw new System.NotImplementedException();
    }

    public void SaveData(Data data)
    {
        data.gridObjects.Add(new AssetSaveObject(origin, rot, gridAsset.assetName));
    }
}
