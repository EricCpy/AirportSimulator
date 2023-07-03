using System.Collections.Generic;
using UnityEngine;

//Objekte des Grids
public class GridObject
{
    private Grid<GridObject> grid;
    private int x, y;
    private PlacedAsset asset;
    public Pathnode node;
    private List<GridObject> neighbours;

    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.node = new Pathnode(grid, x, y);
        this.neighbours = new List<GridObject>();
        this.asset = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + asset;
    }

    public Vector2Int GetOrigin() {
        return new Vector2Int(x,y);
    }

    public void SetPlacedObject(PlacedAsset asset)
    {
        this.asset = asset;
    }

    public void ClearPlacedObject()
    {
        asset = null;
    }

    public PlacedAsset GetPlacedObject()
    {
        return asset;
    }

    public bool CanBuild()
    {
        return asset == null;
    }

    public void AddNeighbour(GridObject neighbour)
    {
        neighbours.Add(neighbour);
        node.AddNeighbour(neighbour.node);
    }

    public void DeleteNeighbour(GridObject neighbour)
    {
        neighbours.Remove(neighbour);
        node.DeleteNeighbour(neighbour.node);
    }

    public List<GridObject> GetNeighbours() { return neighbours; }
}
