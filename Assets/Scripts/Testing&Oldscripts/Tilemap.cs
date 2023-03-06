using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap : MonoBehaviour
{
    private Grid<GridObject> grid;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 10f;
    public Tilemap()
    {
        grid = new Grid<GridObject>(gridWidth, gridHeight, 10f, Vector3.zero, (g, x, y) => new GridObject(g, x, y), true);
    }

    public void SetTileMapSprite(Vector3 pos, GridObject.TilemapSprite sprite) {
        GridObject tilemapObject = grid.GetValue(pos);
        if(tilemapObject != null) {
            //tilemapObject.SetTileMapSprite(sprite);
        }
    }

    public class GridObject {
        public enum TilemapSprite {
            None,
            Ground
        }
        private TilemapSprite tilemapSprite;
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public GridObject(Grid<GridObject> grid, int x, int y){
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        /*public void SetTileMapSprite(TilemapSprite tilemapSprite) {
            this.tilemapSprite = tilemapSprite;
            grid.SetValue(x,y);
        }*/

        public override string ToString()
        {
            return tilemapSprite.ToString();
        }
    }
}
