using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private const int MOVE_STRAIGHT_COST = 1;
    private Grid<Pathnode> grid;
    private List<Pathnode> openList, closedList;
    public Pathfinder(int width, int height)
    {
        grid = new Grid<Pathnode>(width, height, 10f, Vector3.zero, (g, x, y) => new Pathnode(g,x,y), true);
    }

    public Grid<Pathnode> GetGrid() {
        return grid;
    }

    private Pathnode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }

    public List<Pathnode> FindPath(int startX, int startY, int endX, int endY)
    {
        Pathnode startNode = GetNode(startX, startY);
        Pathnode endNode = GetNode(endX, endY);
        openList = new List<Pathnode> { startNode };
        closedList = new List<Pathnode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Pathnode pathnode = GetNode(x, y);
                pathnode.gCost = int.MaxValue;
                pathnode.CalculateFCost();
                pathnode.previous = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            Pathnode current = GetMinFCostNode(openList);
            if (current == endNode)
            {
                return CalculatePath(endNode);
            }
            openList.Remove(current);
            closedList.Add(current);
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour)) continue;
                if(!neighbour.isReachable) {
                    closedList.Add(neighbour);
                    continue;
                }
                int nextGCost = current.gCost + CalculateDistance(current, neighbour);
                if (nextGCost < neighbour.gCost)
                {
                    neighbour.previous = current;
                    neighbour.gCost = nextGCost;
                    neighbour.hCost = CalculateDistance(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour)) openList.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<Pathnode> GetNeighbours(Pathnode node)
    {
        List<Pathnode> neighbours = new List<Pathnode>();
        //left
        if (node.x - 1 >= 0) neighbours.Add(GetNode(node.x - 1, node.y));
        //right
        if (node.x + 1 < grid.GetWidth()) neighbours.Add(GetNode(node.x + 1, node.y));
        //up
        if (node.y + 1 < grid.GetHeight()) neighbours.Add(GetNode(node.x, node.y + 1));
        //down
        if (node.y - 1 >= 0) neighbours.Add(GetNode(node.x, node.y - 1));
        return neighbours;
    }

    private List<Pathnode> CalculatePath(Pathnode endNode)
    {
        List<Pathnode> path = new List<Pathnode>();
        path.Add(endNode);
        Pathnode current = endNode;

        while (current.previous != null)
        {
            path.Add(current.previous);
            current = current.previous;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(Pathnode a, Pathnode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_STRAIGHT_COST * remaining;
    }

    private Pathnode GetMinFCostNode(List<Pathnode> pathNodes)
    {
        Pathnode min = pathNodes[0];
        for (int i = 1; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].fCost < min.fCost) min = pathNodes[i];
        }
        return min;
    }
}
