using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public enum SearchMode
    {
        Dfs,
        Bfs,
        Dijkstra,
        AStar,
        IDAStar,
        DStar,
        Greedy,
    }

    private const int MOVE_STRAIGHT_COST = 1;
    private Grid<GridObject> grid;
    private MinHeap<Pathnode> openList;
    private HashSet<Pathnode> closedList;
    public SearchMode searchMode = SearchMode.AStar;

    public Pathfinder(Grid<GridObject> grid)
    {
        this.grid = grid;
    }

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    private Pathnode GetNode(int x, int y)
    {
        return grid.GetValue(x, y).node;
    }

    public List<Pathnode> FindPath(int startX, int startY, int endX, int endY)
    {
        Pathnode startNode = GetNode(startX, startY);
        Pathnode endNode = GetNode(endX, endY);
        Debug.Log(startNode);
        Debug.Log(endNode);
        List<Pathnode> ans = new List<Pathnode>();
        switch (searchMode)
        {
            case SearchMode.AStar:
                ans = AStarSearch(startNode, endNode);
                break;
            case SearchMode.Dijkstra:
                ans = DijkstraSearch(startNode, endNode);
                break;
            case SearchMode.Dfs:
                ans = DfsSearch(startNode, endNode);
                break;
            case SearchMode.Bfs:
                ans = BfsSearch(startNode, endNode);
                break;
            case SearchMode.Greedy:
                ans = GreedySearch(startNode, endNode);
                break;
        }
        return ans;
    }

    private List<Pathnode> AStarSearch(Pathnode startNode, Pathnode endNode)
    {
        openList = new MinHeap<Pathnode>(grid.MaxSize());
        closedList = new HashSet<Pathnode>();

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
        openList.Add(startNode);

        while (openList.Count > 0)
        {

            Pathnode current = openList.RemoveMin();
            if (current == endNode)
            {
                return CalculatePath(endNode);
            }
            closedList.Add(current);
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour)) continue;
                if (!neighbour.isReachable)
                {
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
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                    else
                    {
                        openList.UpdateItem(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private List<Pathnode> DfsSearch(Pathnode startNode, Pathnode endNode)
    {
        closedList = new HashSet<Pathnode>();
        if (Dfs(startNode, endNode, closedList, 0, int.MaxValue) != -1) return CalculatePath(endNode);
        return null;
    }

    //dfs könnte mit memorization ausgebaut werden, wenn man vom ende startet
    //momentan exponentielle Zeitkomplexität, da schon besuchte Nodes nochmal revisited werden müssen
    private int Dfs(Pathnode current, Pathnode end, HashSet<Pathnode> visited, int pathLength, int minLength)
    {
        if (pathLength >= minLength) return -1;
        if (current == end)
        {
            if (pathLength < minLength)
            {
                return pathLength;
            }
            return -1;
        }
        visited.Add(current);
        Pathnode bestNeighbour = null;
        foreach (Pathnode neighbour in GetNeighbours(current))
        {
            if (closedList.Contains(current)) continue;
            int length = Dfs(neighbour, end, visited, pathLength + 1, minLength);
            if (length != -1 && minLength > length)
            {
                bestNeighbour = neighbour;
                minLength = length;
            }
        }
        visited.Remove(current);
        if (bestNeighbour != null)
        {
            bestNeighbour.previous = current;
            return minLength;
        }
        return -1;
    }

    private List<Pathnode> BfsSearch(Pathnode startNode, Pathnode endNode)
    {
        Queue<Pathnode> queue = new Queue<Pathnode>();
        closedList = new HashSet<Pathnode>();
        queue.Enqueue(startNode);
        closedList.Add(startNode);
        while (queue.Count > 0)
        {
            int size = queue.Count;
            for (int i = 0; i < size; i++)
            {
                Pathnode current = queue.Dequeue();
                foreach (Pathnode neighbour in GetNeighbours(current))
                {
                    if (closedList.Contains(neighbour)) continue;
                    neighbour.previous = current;
                    if (neighbour == endNode) return CalculatePath(endNode);
                    queue.Enqueue(neighbour);
                    closedList.Add(neighbour);
                }
            }
        }
        return null;
    }

    private List<Pathnode> DijkstraSearch(Pathnode startNode, Pathnode endNode)
    {
        HashSet<Pathnode> distances = new HashSet<Pathnode>();
        startNode.fCost = 0;
        distances.Add(startNode);
        openList = new MinHeap<Pathnode>(grid.MaxSize());
        openList.Add(startNode);
        closedList = new HashSet<Pathnode>();
        while (openList.Count > 0)
        {
            Pathnode current = openList.RemoveMin();
            if (current == endNode)
            {
                return CalculatePath(endNode);
            }
            closedList.Add(current);
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour)) continue;
                int nextCost = current.fCost + 1;
                if (!distances.Contains(neighbour) || nextCost < neighbour.fCost)
                {
                    neighbour.fCost = nextCost;
                    neighbour.previous = current;
                    distances.Add(neighbour);
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                    else
                    {
                        openList.UpdateItem(neighbour);
                    }
                }
            }
        }
        return null;
    }

    private List<Pathnode> GreedySearch(Pathnode startNode, Pathnode endNode)
    {
        startNode.fCost = CalculateDistance(startNode, endNode);
        openList = new MinHeap<Pathnode>(grid.MaxSize());
        openList.Add(startNode);
        closedList = new HashSet<Pathnode>();
        while (openList.Count > 0)
        {
            Pathnode current = openList.RemoveMin();
            if (current == endNode)
            {
                return CalculatePath(endNode);
            }
            closedList.Add(current);
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour)) continue;
                neighbour.previous = current;
                neighbour.fCost = CalculateDistance(neighbour, endNode);
                openList.Add(neighbour);

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
