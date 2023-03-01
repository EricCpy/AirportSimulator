using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allgemeine Eigenschaften des Objekts
[CreateAssetMenu()]
public class GridAsset : ScriptableObject
{
    //gets next Rotation from Asset
    public static AssetRotation GetNextAssetRotation(AssetRotation dir)
    {
        switch (dir)
        {
            default:
            case AssetRotation.Down: return AssetRotation.Left;
            case AssetRotation.Left: return AssetRotation.Up;
            case AssetRotation.Up: return AssetRotation.Right;
            case AssetRotation.Right: return AssetRotation.Down;
        }
    }

    //shows in which rotation the asset which will be placed in grid is rotated
    public enum AssetRotation
    {
        Down,
        Left,
        Up,
        Right,
    }
    public Transform prefab;
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private string assetName;

    public Vector2Int GetRotationOffset(AssetRotation rot)
    {
        switch (rot)
        {
            default:
            case AssetRotation.Down: return new Vector2Int(0, 0);
            case AssetRotation.Left: return new Vector2Int(0, width);
            case AssetRotation.Up: return new Vector2Int(width, height);
            case AssetRotation.Right: return new Vector2Int(height, 0);
        }
    }

    public int GetRotationAngle(AssetRotation rot)
    {
        switch (rot)
        {
            default:
            case AssetRotation.Down: return 0;
            case AssetRotation.Left: return 90;
            case AssetRotation.Up: return 180;
            case AssetRotation.Right: return 270;
        }
    }

    public List<Vector2Int> GetPositions(Vector2Int offset, AssetRotation rot)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        switch (rot)
        {
            default:
            case AssetRotation.Down:
            case AssetRotation.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        list.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case AssetRotation.Left:
            case AssetRotation.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        list.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return list;
    }

    public override string ToString()
    {
        return assetName;
    }
}
