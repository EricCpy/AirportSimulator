using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathfidner : MonoBehaviour
{
    // Start is called before the first frame update
    Pathfinder pathfinder;
    [SerializeField] Pathfinder.SearchMode searchMode;
    void Start()
    {
        pathfinder = new Pathfinder(BuildingSystem.Instance.grid);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int xy = pathfinder.GetGrid().GetXY(worldPosition);
            pathfinder.searchMode = searchMode;
            Debug.Log(xy);
            (List<Pathnode>, int) result = pathfinder.FindPath(0,0, xy.x, xy.y);
            var path = result.Item1;
            print(path.Count);
            if(path != null) {
                for(int i = 0; i < path.Count - 1; i++) {
                    //Debug.Log("node:" + new Vector3(path[i].x, path[i].y));
                    //Debug.Log("old:" + new Vector3(path[i+1].x, path[i+1].y));
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + (Vector3.one * 5f), new Vector3(path[i+1].x, path[i+1].y) * 10f + (Vector3.one * 5f), Color.black, 10f);
                }
            }
        }

        if(Input.GetMouseButtonDown(1)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(worldPosition, worldPosition + (Vector3.one * 5f), Color.red, 1000f);
            Vector2Int xy = pathfinder.GetGrid().GetXY(worldPosition);
            Pathnode node = pathfinder.GetGrid().GetValue(xy.x,xy.y).node;
        }
    }
}
