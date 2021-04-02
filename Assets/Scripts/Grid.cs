using UnityEngine;

public class Grid
{
    private int[,] _grid;

    public int this[Vector2Int pos]
    {
        get => _grid[pos.x, pos.y];
        set => _grid[pos.x, pos.y] = value;
    }

    public int this[int x, int y]
    {
        get => _grid[x, y];
        set => _grid[x, y] = value;
    }

    public Grid(int width, int height)
    {
        _grid = new int[width, height];
    }

    public int GetLength(int dimension)
    {
        return _grid.GetLength(dimension);
    }
}
