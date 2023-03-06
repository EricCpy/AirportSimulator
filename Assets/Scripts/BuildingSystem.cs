using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour, IData
{
    public static BuildingSystem Instance { get; private set; }
    private Grid<GridObject> grid;
    [SerializeField] private float cellSize = 10f;
    private GridAsset gridAsset;
    [SerializeField] private List<GridAsset> assetList = null;
    private Dictionary<string, GridAsset> assetDic = new Dictionary<string, GridAsset>();
    private GridAsset.AssetRotation assetRotation;
    public bool deletionMode { get; set; } = false;
    private int[,] dirs4 = new[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
    public void RotateAsset()
    {
        assetRotation = GridAsset.GetNextAssetRotation(assetRotation);
    }

    public void LoadData(Data data)
    {
        grid = new Grid<GridObject>(data.width, data.height, cellSize, Vector3.zero, (g, x, y) => new GridObject(g, x, y), true);
        //Gridassets
        foreach (var loadedAsset in data.gridObjects)
        {
            GridAsset asset;
            assetDic.TryGetValue(loadedAsset.assetName, out asset);
            if (asset != null) PlaceAsset(loadedAsset.origin, loadedAsset.assetRotation, asset);
        }
    }

    public void SaveData(Data data)
    {
        //maybe safe some data later here
    }

    public void SetObjectType(InGameUI.ButtonType type)
    {
        switch (type)
        {
            case InGameUI.ButtonType.Watchtower:
                gridAsset = assetList[0];
                break;
            case InGameUI.ButtonType.Road:
                gridAsset = assetList[1];
                break;
            case InGameUI.ButtonType.Hangar:
                gridAsset = assetList[2];
                break;
            case InGameUI.ButtonType.Planestop:
                gridAsset = assetList[3];
                break;
            case InGameUI.ButtonType.Terminal:
                gridAsset = assetList[4];
                break;
            default:
                gridAsset = null;
                break;
        }

    }
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;
        //needed for loading
        foreach (var obj in assetList)
        {
            assetDic.Add(obj.assetName, obj);
        }
    }

    private void Update()
    {
        if (gridAsset == null && !deletionMode) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int xy = grid.GetXY(worldPosition);
            if (!grid.InBorder(xy)) return;
            if (deletionMode)
            {
                PlacedAsset placedObject = grid.GetValue(worldPosition).GetPlacedObject();
                if (placedObject != null)
                {
                    // Destroy Object in Grid
                    placedObject.DestroyAsset();
                    DeleteObject(placedObject.GetPositions());

                }
                return;
            }

            PlaceAsset(xy, assetRotation, gridAsset);

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int xy = grid.GetXY(worldPosition);
            if (!grid.InBorder(xy)) return;
            PlacedAsset placedObject = grid.GetValue(worldPosition).GetPlacedObject();
            print("aaa");
            print(GetNeighbourAssets(placedObject).Count);
        }

    }

    private void PlaceAsset(Vector2Int xy, GridAsset.AssetRotation assetRot, GridAsset asset)
    {
        List<Vector2Int> gridPositionList = asset.GetPositions(xy, assetRot);
        if (IsClear(gridPositionList, xy))
        {
            Vector2Int rotationOffset = asset.GetRotationOffset(assetRot);
            //rotationOffeset ändern maybe
            Vector3 placedAssetPositon = calculateAssetWorldPositon(xy, rotationOffset);
            PlacedAsset placedAsset = PlacedAsset.Init(placedAssetPositon, xy, assetRot, asset);

            placedAsset.transform.rotation = Quaternion.Euler(0, 0, -asset.GetRotationAngle(assetRot));
            ReserveGrid(gridPositionList, placedAsset);

        }
        else
        {
            Debug.Log("Can not Build here!");
            //TODO: ersetzen durch Schriftzug
        }
    }

    private bool IsClear(List<Vector2Int> gridPositionList, Vector2Int xy)
    {
        foreach (Vector2Int pos in gridPositionList)
        {
            if (!grid.GetValue(pos.x, pos.y).CanBuild())
            {
                return false;
            }
        }
        return true;
    }

    private void ReserveGrid(List<Vector2Int> gridPositionList, PlacedAsset placedAsset)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            grid.GetValue(gridPosition.x, gridPosition.y).SetPlacedObject(placedAsset);
        }
    }

    private void DeleteObject(List<Vector2Int> gridPositionList)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            grid.GetValue(gridPosition.x, gridPosition.y).ClearPlacedObject();
        }
    }

    public List<PlacedAsset> GetNeighbourAssets(PlacedAsset asset)
    {
        if (asset == null) return null;
        List<Vector2Int> positions = asset.GetPositions();
        HashSet<PlacedAsset> neighbours = new HashSet<PlacedAsset>();

        foreach (Vector2Int pos in positions)
        {
            for (int i = 0; i < dirs4.GetLength(0); i++)
            {
                int x = pos.x + dirs4[i, 0];
                int y = pos.y + dirs4[i, 1];
                if (!grid.InBorder(new Vector2Int(x, y))) continue;
                PlacedAsset neighbour = grid.GetValue(x, y).GetPlacedObject();
                if (neighbour != null) Debug.Log(neighbour);
                if (neighbour != null && asset != neighbour)
                {
                    neighbours.Add(neighbour);
                }
            }
        }
        return new List<PlacedAsset>(neighbours);
    }

    public Vector3 calculateAssetWorldPositon(Vector2Int xy, Vector2Int rotationOffset)
    {
        return grid.GetWorldPosition(xy.x, xy.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
    }
}
