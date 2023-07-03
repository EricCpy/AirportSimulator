using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
public class PathfindingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Pathfinder pathfinder;
    private Pathfinder.SearchMode searchMode = Pathfinder.SearchMode.AStar;
    public bool debug = true;
    private int length = 0, searches = 0;
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


    public bool test = false, listCheck =  false;

    // Update is called once per frame
    void Update()
    {
        if (!debug) return;
        if(test) {
            ClearStats();
            test = false;
        }
    }
    
    public void ClearStats() {
        length = 0;
        searches = 0;
        runTime = 0;
    }

    public List<Pathnode> CalculatePath(int startX, int startY, int endX, int endY) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        (List<Pathnode>, int) result = pathfinder.FindPath(startX, startY, endX, endY);
        stopwatch.Stop();
        if(result.Item1 != null) {
            length += result.Item1.Count;
            searches++;
            runTime += stopwatch.ElapsedMilliseconds;
        }
        return result.Item1;
    }

    public float GetAveragePathlength()
    {
        if(searches == 0) return 0;
        return length / (float)searches;
    }

    public float GetAverageRuntime()
    {
        if(searches == 0) return 0;
        return runTime / (float)searches;
    }

    public void ChangeSearchMode(Pathfinder.SearchMode searchMode) {
        pathfinder.searchMode = searchMode;
    }
}

