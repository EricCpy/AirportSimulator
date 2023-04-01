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
    private List<Pathnode> neighbours;
    public Vector2 origin {get; private set;}
    public Vector2Int gridPosition {get; private set;}
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
        this.origin = grid.GetWorldPosition(x, y);
        this.gridPosition = new Vector2Int(x,y);
        this.neighbours = new List<Pathnode>();
        this.isReachable = true;
        this.previous = null;
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

    public void AddNeighbour(Pathnode neighbour)
    {
        neighbours.Add(neighbour);
    }

    public void DeleteNeighbour(Pathnode neighbour)
    {
        neighbours.Remove(neighbour);
    }

    public List<Pathnode> GetNeighbours()
    {
        return neighbours;
    }
}