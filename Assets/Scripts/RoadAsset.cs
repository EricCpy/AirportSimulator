using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadAsset : MonoBehaviour
{
    public PlacedAsset asset;
    public GameObject road1, road2, road3, road4;
    private void Awake()
    {
        asset = GetComponent<PlacedAsset>();
        asset.OnPlacedAsset += OnAssetPlaced;
    }

    private void OnAssetPlaced(object sender, EventArgs e)
    {
        AdaptToNeighbours(true);
    }

    public void AdaptToNeighbours(bool placed)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color oldColor = Color.white;
        if (sr != null) oldColor = sr.color;
        
        List<PlacedAsset> neighbours = BuildingSystem.Instance.GetNeighbourAssets(asset);
        bool left = false, right = false, top = false, bottom = false;
        int count = 0;
        foreach (var neighbour in neighbours)
        {
            RoadAsset road = neighbour.GetComponent<RoadAsset>();
            if (road != null)
            {
                if (placed)
                {
                    BuildingSystem.Instance.AddNeighbourToGridObject(asset.origin, neighbour.origin);

                    road.AdaptToNeighbours(false);
                }
                Vector2Int pos = asset.origin - neighbour.origin;
                if (pos.y == -1) top = true;
                else if (pos.y == 1) bottom = true;
                else if (pos.x == -1) right = true;
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
            if (!bottom) asset.SetRotation(GridAsset.AssetRotation.Down);
            else if (!top) asset.SetRotation(GridAsset.AssetRotation.Up);
            else if (!left) asset.SetRotation(GridAsset.AssetRotation.Left);
            else if (!right) asset.SetRotation(GridAsset.AssetRotation.Right);
        }
        else if (count == 2)
        {
            if (left && right)
            {
                road1.SetActive(true);
                asset.SetRotation(GridAsset.AssetRotation.Left);
            }
            else if (bottom && top)
            {
                road1.SetActive(true);
                asset.SetRotation(GridAsset.AssetRotation.Up);
            }
            else
            {
                road2.SetActive(true);

                if (left && bottom)
                {
                    asset.SetRotation(GridAsset.AssetRotation.Down);
                }
                else if (left && top)
                {
                    asset.SetRotation(GridAsset.AssetRotation.Left);
                }
                else if (right && bottom)
                {
                    asset.SetRotation(GridAsset.AssetRotation.Right);
                }
                else
                {
                    //right && top == up
                    asset.SetRotation(GridAsset.AssetRotation.Up);
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
            else
            {
                asset.SetRotation(GridAsset.AssetRotation.Up);
            }
        }
        GetComponentInChildren<SpriteRenderer>().color = oldColor;
    }

    private void SetRoadsInactive()
    {
        road4.SetActive(false);
        road3.SetActive(false);
        road2.SetActive(false);
        road1.SetActive(false);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || GameManager.Instance.uiOpen) return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            //anfang runway
            AirportManager.Instance.SetRunwayStart(BuildingSystem.Instance.MousePositionToGridPosition(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //ende runway
            AirportManager.Instance.SetRunwayEnd(BuildingSystem.Instance.MousePositionToGridPosition(Input.mousePosition));
        }
    }

    private void OnDestroy()
    {
        List<PlacedAsset> neighbours = BuildingSystem.Instance.GetNeighbourAssets(asset);
        foreach (var neighbour in neighbours)
        {
            RoadAsset road = neighbour.GetComponent<RoadAsset>();
            if (road != null)
            {
                BuildingSystem.Instance.DeleteNeighbourFromGridObject(asset.origin, neighbour.origin);
                road.AdaptToNeighbours(false);
            }
        }
    }
}
