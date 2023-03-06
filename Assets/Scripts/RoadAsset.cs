using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadAsset : MonoBehaviour
{
    private PlacedAsset asset;
    public GameObject road1, road2, road3, road4;
    private void Start()
    {
        asset = GetComponent<PlacedAsset>();
        transform.rotation = Quaternion.identity;
        AdaptToNeighbours(true);
    }


    public void AdaptToNeighbours(bool placed)
    {
        List<PlacedAsset> neighbours = BuildingSystem.Instance.GetNeighbourAssets(asset);
        bool left = false, right = false, top = false, bottom = false;
        int count = 0;
        foreach (var neighbour in neighbours)
        {
            RoadAsset road = neighbour.GetComponent<RoadAsset>();
            if (road != null)
            {
                if (placed) road.AdaptToNeighbours(false);
                Vector2Int pos = neighbour.origin;
                if (pos.y == 1) top = true;
                else if (pos.y == -1) bottom = true;
                else if (pos.x == 1 && pos.y == 0) right = true;
                else left = true;
                count++;
            }
        }

        SetRoadsInactive();
        if (count == 4)
        {
            road4.SetActive(true);
        }
        else if (count == 3)
        {
            road3.SetActive(true);
            if (!bottom) road3.transform.localRotation = Quaternion.Euler(0, 0, 0);
            else if (!top) road3.transform.localRotation = Quaternion.Euler(0, 0, 180);
            else if (!left) road3.transform.localRotation = Quaternion.Euler(0, 0, 270);
            else if (!right) road3.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (count == 2)
        {
            if (left && right)
            {
                road1.SetActive(true);
                road1.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (bottom && top)
            {
                road1.SetActive(true);
            }
            else
            {
                road2.SetActive(true);
                if (left && top)
                {
                    road2.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                else if (right && top)
                {
                    road2.transform.localRotation = Quaternion.Euler(0, 0, 270);
                }
                else if (right && bottom)
                {
                    road2.transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
            }
        }
        else
        {
            road1.SetActive(true);
            if (left || right)
            {
                asset.SetRotation(GridAsset.AssetRotation.Left);
            }
        }
    }

    private void SetRoadsInactive()
    {
        road4.SetActive(false);
        road3.SetActive(false);
        road2.SetActive(false);
        road1.SetActive(false);
    }
}
