using System;
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
    public Vector2Int origin { get; private set; }
    public GridAsset.AssetRotation rot;
    public event EventHandler OnPlacedAsset;

    private void Create(GridAsset gridAsset, Vector2Int origin, GridAsset.AssetRotation rot)
    {
        this.gridAsset = gridAsset;
        this.origin = origin;
        this.rot = rot;
        OnPlacedAsset += OnAssetPlaced;
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
        data.gridObjects.Add(new AssetSaveObject(origin, rot, gridAsset.assetName));
    }

    public void SetRotation(GridAsset.AssetRotation rot)
    {
        this.rot = rot;
        transform.position = BuildingSystem.Instance.calculateAssetWorldPositon(origin, gridAsset.GetRotationOffset(rot));
        transform.rotation = Quaternion.Euler(0, 0, -gridAsset.GetRotationAngle(rot));
    }

    public float GetRotation()
    {
        return gridAsset.GetRotationAngle(rot);
    }

    public void SetNextRotation()
    {
        this.rot = GridAsset.GetNextAssetRotation(rot);
        transform.position = BuildingSystem.Instance.calculateAssetWorldPositon(origin, gridAsset.GetRotationOffset(rot));
        transform.rotation = Quaternion.Euler(0, 0, -gridAsset.GetRotationAngle(rot));
    }

    public void TriggerPlacedAsset()
    {
        OnPlacedAsset?.Invoke(this, EventArgs.Empty);
    }

    private void OnAssetPlaced(object sender, EventArgs e)
    {
        foreach (var position in GetPositions())
        {
            foreach (var neighbour in BuildingSystem.Instance.grid.GetValue(position.x, position.y).GetNeighbours())
            {
                RoadAsset road = neighbour.GetPlacedObject().GetComponent<RoadAsset>();
                if (road != null)
                {
                    road.AdaptToNeighbours();
                }
            }
        }
    }
    private void OnDestroy()
    {
        foreach (var position in GetPositions())
        {
            foreach (var neighbour in BuildingSystem.Instance.grid.GetValue(position.x, position.y).GetNeighbours())
            {
                RoadAsset road = neighbour.GetPlacedObject().GetComponent<RoadAsset>();
                if (road != null)
                {
                    BuildingSystem.Instance.DeleteNeighbourFromGridObject(position, neighbour.GetPosition());
                    road.AdaptToNeighbours();
                }
            }
        }
    }
    public GridAsset GetGridAsset()
    {
        return gridAsset;
    }
}
