using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = new Tilemap();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            tilemap.SetTileMapSprite(worldPosition, Tilemap.GridObject.TilemapSprite.Ground);
        }
    }

}
