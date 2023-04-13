using System;
using UnityEngine;

public class Grid<T>
{
    private int width, height;
    private float cellSize;
    private T[,] grid;
    private Vector3 originPosition;
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<T>, int, int, T> createObject, bool debug = false)
    {
        this.width = width;
        this.height = height;
        grid = new T[width, height];
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = createObject(this, x, y);
            }
        }

        if (debug)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    GameObject obj = new GameObject("GENERATED_TXT", typeof(TextMesh));
                    obj.transform.localPosition = GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
                    TextMesh txt = obj.GetComponent<TextMesh>();
                    txt.text = grid[x, y].ToString();
                    txt.anchor = TextAnchor.MiddleCenter;
                    txt.fontSize = 40;
                    txt.GetComponent<MeshRenderer>().sortingOrder = 10;
                }
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        Vector2Int xy =new Vector2Int(Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize), Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellSize));
        if(InBorder(xy)) return xy;
        return new Vector2Int(-1, -1);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= grid.GetLength(0) || y >= grid.GetLength(1) || x < 0 || y < 0) return;
        grid[x, y] = value;
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int xy = GetXY(worldPosition);
        SetValue(xy.x, xy.y, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= grid.GetLength(0) || y >= grid.GetLength(1) || x < 0 || y < 0) return default(T);
        return grid[x, y];
    }

    public T GetValue(Vector3 worldPosition)
    {
        Vector2Int xy = GetXY(worldPosition);
        return GetValue(xy.x, xy.y);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public int MaxSize()
    {
        return height * width;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public bool InBorder(Vector2Int xy) {
        int x = xy.x;
        int y = xy.y;
        if (x >= grid.GetLength(0) || y >= grid.GetLength(1) || x < 0 || y < 0) return false;
        return true;
    }
}
