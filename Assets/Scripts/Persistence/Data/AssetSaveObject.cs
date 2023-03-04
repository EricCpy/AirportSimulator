using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetSaveObject
{
    public string assetName;
    public GridAsset.AssetRotation assetRotation;
    public Vector2Int origin; 

    public AssetSaveObject(Vector2Int origin, GridAsset.AssetRotation assetRotation, string assetName) {
        this.origin = origin;
        this.assetRotation = assetRotation;
        this.assetName = assetName;
    }
}
