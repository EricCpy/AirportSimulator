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
        DStar,
        Greedy,
        Ids,
        Dfs_Memorization
    }

    private const int MOVE_STRAIGHT_COST = 1;
    private Grid<GridObject> grid;
    private HashSet<Pathnode> closedList;
    public SearchMode searchMode = SearchMode.AStar;
    private MinHeap<Pathnode> openList;
    public Pathfinder(Grid<GridObject> grid)
    {
        this.grid = grid;
        closedList = new HashSet<Pathnode>();
        openList = new MinHeap<Pathnode>(grid.MaxSize());
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
        startNode.previous = null;
        endNode.previous = null;
        //es gibt noch andere Algorithmen wie ida* oder idgreedy, diese vereinigen die unteren algorithmen und verbessern diese teilweise
        bool ans = false;
        switch (searchMode)
        {
            case SearchMode.AStar:
                ans = AStarSearch(startNode, endNode);
                break;
            case SearchMode.Dijkstra:
                ans = DijkstraSearch(startNode, endNode);
                break;
            case SearchMode.Dfs_Memorization:
            case SearchMode.Dfs:
                ans = DepthFirstSearch(startNode, endNode);
                break;
            case SearchMode.Bfs:
                ans = BfsSearch(startNode, endNode);
                break;
            case SearchMode.Greedy:
                ans = GreedySearch(startNode, endNode);
                break;
            case SearchMode.Ids:
                ans = IterativeDeepeningSearch(startNode, endNode);
                break;
        }
        List<Pathnode> list = new List<Pathnode>();
        if (ans) list = CalculatePath(endNode);
        ClearVisitedNodes();
        return list;
    }

    private void ClearVisitedNodes()
    {
        while (openList.Count > 0)
        {
            closedList.Add(openList.RemoveMin());
        }
        foreach (var node in closedList)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.CalculateFCost();
            node.previous = null;
        }
        closedList.Clear();
    }

    private bool IterativeDeepeningSearch(Pathnode startNode, Pathnode endNode)
    {
        for (int i = 1; i < grid.MaxSize() / 2 + Mathf.Max(grid.GetWidth(), grid.GetHeight()); i++)
        {
            if (LimitedDfs(startNode, endNode, 0, i)) return true;
        }
        return false;
    }

    private bool LimitedDfs(Pathnode current, Pathnode end, int currLength, int lengthLimit)
    {
        if (currLength >= lengthLimit) return false;
        if (current == end) return true;

        closedList.Add(current);
        foreach (Pathnode neighbour in GetNeighbours(current))
        {
            if (closedList.Contains(neighbour)) continue;
            if (LimitedDfs(neighbour, end, currLength + 1, lengthLimit))
            {
                neighbour.previous = current;
                return true;
            }
        }
        closedList.Remove(current);
        return false;
    }

    private bool AStarSearch(Pathnode startNode, Pathnode endNode)
    {
        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            Pathnode current = openList.RemoveMin();
            closedList.Add(current);
            if (current == endNode) return true;
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour)) continue;
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

        return false;
    }

    //normale dfs kann kürzesten path nicht finden, sondern würde irgendeinen finden
    private bool DepthFirstSearch(Pathnode current, Pathnode end)
    {
        if (searchMode == SearchMode.Dfs && Dfs(current, end, 0, int.MaxValue) != -1) return true;
        if (searchMode == SearchMode.Dfs_Memorization && Dfs_Memorization(current, end, 0, int.MaxValue, new Dictionary<Pathnode, int>()) != -1) return true;
        return false;
    }

    private int Dfs_Memorization(Pathnode current, Pathnode end, int currLength, int minLength, Dictionary<Pathnode, int> memo)
    {
        if (currLength >= minLength) return -1;
        if (current == end) return currLength;
        if (memo.ContainsKey(current))
        {
            if (memo[current] == -1) return -1;
            return currLength + memo[current];
        }

        closedList.Add(current);
        Pathnode bestNeighbour = null;
        foreach (Pathnode neighbour in GetNeighbours(current))
        {
            if (closedList.Contains(neighbour)) continue;
            int length = Dfs_Memorization(neighbour, end, currLength + 1, minLength, memo);
            if (length != -1 && minLength > length)
            {
                bestNeighbour = neighbour;
                minLength = length;
            }
        }
        closedList.Remove(current);
        if (bestNeighbour != null)
        {
            memo[current] = minLength - currLength;
            bestNeighbour.previous = current;
            return minLength;
        }
        memo[current] = -1;
        return -1;
    }

    //dfs könnte mit memorization ausgebaut werden, wenn man vom ende startet
    //momentan exponentielle Zeitkomplexität, da schon besuchte Nodes nochmal revisited werden müssen
    private int Dfs(Pathnode current, Pathnode end, int currLength, int minLength)
    {
        if (currLength >= minLength) return -1;
        if (current == end) return currLength;

        closedList.Add(current);
        Pathnode bestNeighbour = null;
        foreach (Pathnode neighbour in GetNeighbours(current))
        {
            if (closedList.Contains(neighbour)) continue;
            int length = Dfs(neighbour, end, currLength + 1, minLength);
            if (length != -1 && minLength > length)
            {
                bestNeighbour = neighbour;
                minLength = length;
            }
        }
        closedList.Remove(current);
        if (bestNeighbour != null)
        {
            bestNeighbour.previous = current;
            return minLength;
        }
        return -1;
    }

    private bool BfsSearch(Pathnode startNode, Pathnode endNode)
    {
        Queue<Pathnode> queue = new Queue<Pathnode>();
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
                    if (neighbour == endNode) return true;
                    queue.Enqueue(neighbour);
                    closedList.Add(neighbour);
                }
            }
        }
        return false;
    }

    private bool DijkstraSearch(Pathnode startNode, Pathnode endNode)
    {
        HashSet<Pathnode> distances = new HashSet<Pathnode>();
        startNode.fCost = 0;
        distances.Add(startNode);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            Pathnode current = openList.RemoveMin();
            if (current == endNode)
            {
                return true;
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
        return false;
    }

    private bool GreedySearch(Pathnode startNode, Pathnode endNode)
    {
        startNode.fCost = CalculateDistance(startNode, endNode);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            Pathnode current = openList.RemoveMin();
            if (current == endNode)
            {
                return true;
            }
            closedList.Add(current);
            foreach (Pathnode neighbour in GetNeighbours(current))
            {
                if (closedList.Contains(neighbour) || openList.Contains(neighbour)) continue;
                neighbour.previous = current;
                neighbour.fCost = CalculateDistance(neighbour, endNode);
                openList.Add(neighbour);
            }
        }

        return false;
    }

    private List<Pathnode> GetNeighbours(Pathnode node)
    {
        List<Pathnode> neighbours = node.GetNeighbours();
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

    //Manhattan Distance
    private int CalculateDistance(Pathnode a, Pathnode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int distance = xDistance + yDistance;
        return distance;
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
