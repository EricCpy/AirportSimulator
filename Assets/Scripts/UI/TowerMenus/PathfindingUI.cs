using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text averageRuntime;
    [SerializeField] private TMP_Text averagePathlength;
    [SerializeField] private TMP_Text averageTraversedNodes;
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
                searchMode = Pathfinder.SearchMode.AStar;
                break;
            case 1:
                searchMode = Pathfinder.SearchMode.Dfs;
                break;
            case 2:
                searchMode = Pathfinder.SearchMode.Bfs;
                break;
            case 3:
                searchMode = Pathfinder.SearchMode.Dijkstra;
                break;
            case 4:
                searchMode = Pathfinder.SearchMode.Greedy;
                break;
            case 5:
                searchMode = Pathfinder.SearchMode.Ids;
                break;
            case 6:
                searchMode = Pathfinder.SearchMode.Dfs_Memorization;
                break;
        }
        PathfindingManager.Instance.ChangeSearchMode(searchMode);
    }

    private IEnumerator UpdateAverages(float secs)
    {
        var delay = new WaitForSecondsRealtime(secs);
        while (true)
        {
            averageRuntime.text = PathfindingManager.Instance.GetAverageRuntime().ToString("0.00000000") + " ms";
            averagePathlength.text = PathfindingManager.Instance.GetAveragePathlength().ToString("0.00") + " Nodes";
            averageTraversedNodes.text = PathfindingManager.Instance.GetAverageTraversedNodes().ToString("0.00") + " Nodes";
            yield return delay;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void RecalculatePaths()
    {
        AirportManager.Instance.RecalculatePaths();
    }
}
