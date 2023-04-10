using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour, IData
{
    public static BuildingSystem Instance { get; private set; }
    public Grid<GridObject> grid;
    [SerializeField] private float cellSize = 10f;
    private GridAsset gridAsset;
    [SerializeField] private List<GridAsset> assetList = null;
    private Dictionary<string, GridAsset> assetDic = new Dictionary<string, GridAsset>();
    private GridAsset.AssetRotation assetRotation;
    public bool deletionMode { get; set; } = false;
    private int[,] dirs4 = new[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
    public bool assetsLoaded { get; private set; }
    private bool helperLines = true;
    [SerializeField] private Transform lineContainer;
    [SerializeField] private GameObject linePrefab;
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
            if (asset != null)
            {
                PlaceAsset(loadedAsset.origin, loadedAsset.assetRotation, asset);
            }
        }
        helperLines = data.helperLines;
        InitalizeLines();
        lineContainer.gameObject.SetActive(helperLines);
    }

    public void ToggleGrid()
    {
        helperLines = !helperLines;
        lineContainer.gameObject.SetActive(helperLines);
    }

    private void InitalizeLines()
    {
        for (int x = 0; x <= grid.GetWidth(); x++)
        {
            GameObject lineObject = Instantiate(linePrefab);
            lineObject.transform.SetParent(lineContainer);
            lineObject.transform.localScale = new Vector3(.5f, grid.GetHeight() * cellSize, 1);
            lineObject.transform.position = new Vector3(x * cellSize, grid.GetHeight() * cellSize / 2, 0);
        }

        for (int y = 0; y <= grid.GetHeight(); y++)
        {
            GameObject lineObject = Instantiate(linePrefab);
            lineObject.transform.SetParent(lineContainer);
            lineObject.transform.localScale = new Vector3(grid.GetWidth() * cellSize, .5f, 1);
            lineObject.transform.position = new Vector3(grid.GetWidth() * cellSize / 2, y * cellSize, 0);
        }
    }

    public void SaveData(Data data)
    {
        data.helperLines = helperLines;
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

    public Vector2Int MousePositionToGridPosition(Vector3 mousePosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2Int xy = grid.GetXY(worldPosition);
        return xy;
    }

    public PlacedAsset GetObjectFromMousePostion(Vector3 mousePosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return grid.GetValue(worldPosition).GetPlacedObject();
    }

    private void Update()
    {
        if ((gridAsset == null && !deletionMode) || EventSystem.current.IsPointerOverGameObject() || GameManager.Instance.uiOpen) return;
        if (Input.GetMouseButtonDown(0))
        {
            assetsLoaded = true;
            Vector2Int xy = MousePositionToGridPosition(Input.mousePosition);
            if (!grid.InBorder(xy)) return;
            if (deletionMode)
            {
                PlacedAsset placedObject = GetObjectFromMousePostion(Input.mousePosition);
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
    }

    private void PlaceAsset(Vector2Int xy, GridAsset.AssetRotation assetRot, GridAsset asset)
    {
        List<Vector2Int> gridPositionList = asset.GetPositions(xy, assetRot);
        if (IsClear(gridPositionList, xy))
        {
            Vector2Int rotationOffset = asset.GetRotationOffset(assetRot);
            Vector3 placedAssetPositon = calculateAssetWorldPositon(xy, rotationOffset);
            PlacedAsset placedAsset = PlacedAsset.Init(placedAssetPositon, xy, assetRot, asset);

            ReserveGrid(gridPositionList, placedAsset);
            placedAsset.TriggerPlacedAsset();
        }
        else
        {
            Debug.Log("Can not Build here!");
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
        return grid.GetWorldPosition(xy.x, xy.y) + new Vector3(rotationOffset.x, rotationOffset.y) * cellSize;
    }

    public void AddNeighbourToGridObject(Vector2Int xy1, Vector2Int xy2)
    {
        GridObject object1 = grid.GetValue(xy1.x, xy1.y);
        GridObject object2 = grid.GetValue(xy2.x, xy2.y);
        object1.AddNeighbour(object2);
        object2.AddNeighbour(object1);
    }

    public void DeleteNeighbourFromGridObject(Vector2Int xy1, Vector2Int xy2)
    {
        GridObject object1 = grid.GetValue(xy1.x, xy1.y);
        GridObject object2 = grid.GetValue(xy2.x, xy2.y);
        object1.DeleteNeighbour(object2);
        object2.DeleteNeighbour(object1);
    }

    public float GetCellSize() { return cellSize; }
}
