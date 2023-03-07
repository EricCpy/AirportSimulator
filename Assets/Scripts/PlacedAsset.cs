using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//platziertes Asset
public class PlacedAsset : MonoBehaviour, IData
{
    public static PlacedAsset Init(Vector3 worldPos, Vector2Int origin, GridAsset.AssetRotation rot, GridAsset gridAsset)
    {
        Transform assetTransform = Instantiate(gridAsset.prefab, worldPos, Quaternion.Euler(0, 0, -gridAsset.GetRotationAngle(rot)));
        PlacedAsset asset = assetTransform.GetComponent<PlacedAsset>();
        asset.Create(gridAsset, origin, rot);
        return asset;
    }

    private GridAsset gridAsset;
    public Vector2Int origin {get; private set;}
    public GridAsset.AssetRotation rot;

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
        //no implementation bc building is done in BuildingSystem
    }

    public void SaveData(Data data)
    {
        Debug.Log("save asset");
        data.gridObjects.Add(new AssetSaveObject(origin, rot, gridAsset.assetName));
    }

    public void SetRotation(GridAsset.AssetRotation rot) {
        this.rot = rot;
        transform.position = BuildingSystem.Instance.calculateAssetWorldPositon(origin, gridAsset.GetRotationOffset(rot));
        transform.rotation = Quaternion.Euler(0, 0, -gridAsset.GetRotationAngle(rot));
    }

    public float GetRotation() {
        return gridAsset.GetRotationAngle(rot);
    }

    public void SetNextRotation() {
        this.rot = GridAsset.GetNextAssetRotation(rot);
        transform.position = BuildingSystem.Instance.calculateAssetWorldPositon(origin, gridAsset.GetRotationOffset(rot));
        transform.rotation = Quaternion.Euler(0, 0, -gridAsset.GetRotationAngle(rot));
    }
}
