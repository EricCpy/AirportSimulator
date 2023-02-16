using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode
{
    private Grid<Pathnode> grid;
    public int x, y;
    public int gCost;
    public int hCost;
    public int fCost;
    public Pathnode previous;
    public bool isReachable;
    public Pathnode(Grid<Pathnode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isReachable = true;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }
}