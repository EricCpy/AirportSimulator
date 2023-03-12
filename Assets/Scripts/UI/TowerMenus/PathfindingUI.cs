using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text averageRuntime;
    [SerializeField] private TMP_Text averagePathlength;
    [SerializeField] private Toggle debug;
    [SerializeField] private TMP_Dropdown algorithm;
    private void OnEnable()
    {
        StartCoroutine(UpdateAverages(2));
    }

    public void ChangePathfindingAlgorithm()
    {
        Pathfinder.SearchMode searchMode = Pathfinder.SearchMode.Greedy;
        switch (algorithm.value)
        {
            case 0:
                searchMode = Pathfinder.SearchMode.Greedy;
                break;
            case 1:
                searchMode = Pathfinder.SearchMode.Dfs;
                break;
            case 2:
                Debug.Log("BFS");
                searchMode = Pathfinder.SearchMode.Bfs;
                break;
            case 3:
                searchMode = Pathfinder.SearchMode.Dijkstra;
                break;
            case 4:
                searchMode = Pathfinder.SearchMode.AStar;
                break;
            case 5:
                searchMode = Pathfinder.SearchMode.Ids;
                break;
        }
        PathfindingManager.Instance.ChangeSearchMode(searchMode);
    }

    public void ToggleDebug()
    {
        PathfindingManager.Instance.debug = !PathfindingManager.Instance.debug;
    }


    private IEnumerator UpdateAverages(float secs)
    {
        var delay = new WaitForSecondsRealtime(secs);
        while (true)
        {
            averageRuntime.text = PathfindingManager.Instance.GetAverageRuntime() + "s";
            averagePathlength.text = PathfindingManager.Instance.GetAveragePathlength() + "Nodes";
            yield return delay;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
