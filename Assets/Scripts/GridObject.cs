using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Objekte des Grids
public class GridObject
{
    private Grid<GridObject> grid;
    private int x, y;
    public PlacedAsset asset;
    public Pathnode node;
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.node = new Pathnode(grid, x, y);
        this.asset = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + asset;
    }

    public void SetPlacedObject(PlacedAsset asset)
    {
        this.asset = asset;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearPlacedObject()
    {
        asset = null;
        grid.TriggerGridObjectChanged(x, y);
    }

    public PlacedAsset GetPlacedObject()
    {
        return asset;
    }

    public bool CanBuild()
    {
        return asset == null;
    }
}
