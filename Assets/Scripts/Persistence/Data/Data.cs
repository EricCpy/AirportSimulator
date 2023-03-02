using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public Grid<GridObject> grid;
    public string hello = "hello";
    public Data() {
        grid = new Grid<GridObject>(10, 10, 10f, Vector3.zero, (g, x, y) => new GridObject(g, x, y), true);
    }
}
