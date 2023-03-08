using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode : IMinHeapItem<Pathnode>
{
    private Grid<GridObject> grid;
    public int x, y;
    public int gCost;
    public int hCost;
    public int fCost;
    public Pathnode previous;
    public bool isReachable;
    private int minHeapIndex;
    public int MinHeapIndex
    {
        get { return minHeapIndex; }
        set { minHeapIndex = value; }
    }

    public Pathnode(Grid<GridObject> grid, int x, int y)
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

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public int CompareTo(Pathnode other)
    {
        int compare = this.fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }

    public bool Equals(Pathnode other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return x == other.x && y == other.y;
    }
}