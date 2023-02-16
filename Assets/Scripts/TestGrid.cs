using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    private Grid<bool> grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<bool>(20, 10, 0.5f, new Vector3(-11, -5), (g, x, y) => false, true); 
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grid.SetValue(worldPosition, true);
            Debug.Log(100);
        }

        if(Input.GetMouseButtonDown(1)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(grid.GetValue(worldPosition));
        }
    }

}
