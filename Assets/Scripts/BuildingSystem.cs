using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private Transform building;
    private Grid<Pathnode> grid;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 10f;
    private void Awake()
    {
        grid = new Grid<Pathnode>(gridWidth, gridHeight, 10f, Vector3.zero, (g, x, y) => new Pathnode(g,x,y), true);
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(building, worldPosition, Quaternion.identity);
        }
    }

    public class GridObject {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public GridObject(Grid<GridObject> grid, int x, int y){
            this.grid = grid;
            this.x = x;
            this.y = y;
        }
    }
}
