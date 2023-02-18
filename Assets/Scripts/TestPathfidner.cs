using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathfidner : MonoBehaviour
{
    // Start is called before the first frame update
    Pathfinder pathfinder;
    void Start()
    {
        pathfinder = new Pathfinder(10,10);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int xy = pathfinder.GetGrid().GetXY(worldPosition);
            Debug.Log(xy);
            List<Pathnode> path = pathfinder.FindPath(0,0, xy.x, xy.y);
            if(path != null) {
                for(int i = 0; i < path.Count - 1; i++) {
                    Debug.Log("old:" + new Vector3(path[i].x, path[i].y));
                    Debug.Log("old:" + new Vector3(path[i+1].x, path[i+1].y));
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + (Vector3.one * 5f), new Vector3(path[i+1].x, path[i+1].y) * 10f + (Vector3.one * 5f), Color.black, 5f);
                }
            }
        }

        if(Input.GetMouseButtonDown(1)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(worldPosition, worldPosition + (Vector3.one * 5f), Color.red, 1000f);
            Vector2Int xy = pathfinder.GetGrid().GetXY(worldPosition);
            Pathnode node = pathfinder.GetGrid().GetValue(xy.x,xy.y);
            node.isReachable = false;
        }
    }
}
