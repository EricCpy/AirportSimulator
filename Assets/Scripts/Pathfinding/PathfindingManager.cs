using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
public class PathfindingManager : MonoBehaviour
{
    // Start is called before the first frame update
    Pathfinder pathfinder;
    private Pathfinder.SearchMode searchMode = Pathfinder.SearchMode.Greedy;
    public bool debug = true;
    private int length = 0, searches = 0;
    private float runTime = 0f;
    public static PathfindingManager Instance { get; private set; }
    public bool test;
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

    // Update is called once per frame
    void Update()
    {
        if (!debug) return;
        if (Input.GetMouseButtonDown(0) && test)
        {
            Vector2Int xy = BuildingSystem.Instance.MousePositionToGridPosition(Input.mousePosition);
            Debug.Log(xy);
            List<Pathnode> path = CalculatePath(0, 0, xy.x, xy.y);
            ActiveVehicle.Init(VehicleManager.Instance.taxi, path, false);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    //Debug.Log("node:" + new Vector3(path[i].x, path[i].y));
                    //Debug.Log("old:" + new Vector3(path[i+1].x, path[i+1].y));
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + (Vector3.one * 5f), new Vector3(path[i + 1].x, path[i + 1].y) * 10f + (Vector3.one * 5f), Color.black, 10f);
                }
            }
        }
    }

    public List<Pathnode> CalculatePath(int startX, int startY, int endX, int endY) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        List<Pathnode> path = pathfinder.FindPath(startX, startY, endX, endY);
        stopwatch.Stop();
        if(path != null) {
            length += path.Count;
            searches++;
            runTime += stopwatch.ElapsedMilliseconds / 1000;
        }
        return path;
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

