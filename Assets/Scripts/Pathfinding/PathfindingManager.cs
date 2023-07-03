using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
public class PathfindingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Pathfinder pathfinder;
    private Pathfinder.SearchMode searchMode = Pathfinder.SearchMode.AStar;
    private long length = 0, searches = 0, traversedNodes = 0;
    private float runTime = 0f;
    public static PathfindingManager Instance { get; private set; }
    public event Action OnInitialized;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("Buildingsystem has already an Instance");
        }
        Instance = this;
    }

    void Start()
    {
        pathfinder = new Pathfinder(BuildingSystem.Instance.grid);
        OnInitialized?.Invoke();
    }

    public void ClearStats()
    {
        length = 0;
        searches = 0;
        runTime = 0;
        traversedNodes = 0;
    }

    public List<Pathnode> CalculatePath(int startX, int startY, int endX, int endY)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        (List<Pathnode>, int) result = pathfinder.FindPath(startX, startY, endX, endY);
        stopwatch.Stop();
        if (result.Item1 != null)
        {
            traversedNodes += result.Item2;
            length += result.Item1.Count;
            searches++;
            runTime += stopwatch.ElapsedMilliseconds;
        }
        return result.Item1;
    }

    public float GetAveragePathlength()
    {
        if (searches == 0) return 0;
        return length / (float)searches;
    }

    public float GetAverageRuntime()
    {
        if (searches == 0) return 0;
        return runTime / (float)searches;
    }

    public float GetAverageTraversedNodes()
    {
        if (searches == 0) return 0;
        return traversedNodes / (float)searches;
    }

    public void ChangeSearchMode(Pathfinder.SearchMode searchMode)
    {
        pathfinder.searchMode = searchMode;
    }
}

