using UnityEngine;

public class PlayArea : MonoBehaviour
{
    private const int EMPTY = 0;
    private const int MOVING_PIECE = 1;
    private const int STATIC_PIECE = 2;
    private const int BORDER = 3;

    [SerializeField, Range(0f, 50f)]
    private int width = 20;

    [SerializeField, Range(0f, 50f)]    
    private int height = 50;

    [SerializeField]
    private float cellSize = 1f;

    /// <summary>
    /// 0 - empty
    /// 1 - moving piece
    /// 2 - static piece
    /// 3 - border
    /// </summary>
    private int[,] _grid = null;

    private Vector2 _BottomLeftCorner => transform.position - new Vector3(cellSize * width * 0.5f, cellSize * height * 0.5f, 0f);
    private Vector2 _TopRightCorner => transform.position + new Vector3(cellSize * width * 0.5f, cellSize * height * 0.5f, 0f);

    private Rect _Extentents => new Rect(_BottomLeftCorner, new Vector2(width * cellSize, height * cellSize));


    private void Awake()
    {
        InitializeGrid();
    }

    private void OnValidate()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        _grid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[x, y] = EMPTY;
            }
        }
        for (int x = 0; x < width; x++)
        {
            _grid[x, 0] = 3;
            _grid[x, height - 1] = BORDER;
        }
        for (int y = 0; y < height; y++)
        {
            _grid[0, y] = 3;
            _grid[width - 1, y] = BORDER;
        }
    }

    public float GetDistanceToCollision(Vector2 origin, Vector2Int direction)
    {
        Vector2Int originGrid = PositionToGrid(origin);

        int gridDistance = DistanceToNextOccupiedGrid(originGrid, direction);

        float distanceToGridCenter = (origin - _BottomLeftCorner - Vector2.one * cellSize * 0.5f - new Vector2(originGrid.x * cellSize, originGrid.y * cellSize)).magnitude;

        return distanceToGridCenter + gridDistance * cellSize;
    }

    private int DistanceToNextOccupiedGrid(Vector2Int gridOriginIdx, Vector2Int directionNormalized)
    {
        int distance = 0;
        while(_grid[gridOriginIdx.x + directionNormalized.x, gridOriginIdx.y + directionNormalized.y] == EMPTY)
        {
            distance++;
            gridOriginIdx += directionNormalized;
        }
        return distance;
    }

    public Vector2Int PositionToGrid(Vector2 position)
    {
        Debug.Assert(position.x >= _BottomLeftCorner.x + cellSize * 0.5f &&
                     position.y >= _BottomLeftCorner.y + cellSize * 0.5f &&
                     position.x <= _TopRightCorner.x - cellSize * 0.5f &&
                     position.y <= _TopRightCorner.y - cellSize * 0.5f);

        Vector2 relativePosition = position - _BottomLeftCorner;

        return new Vector2Int((int)relativePosition.x / (int)cellSize, (int)relativePosition.y / (int)cellSize);
    }

    public void MarkStaticPieces(Transform[] positions)
    {
        foreach(Transform piece in positions)
        {
            Vector2Int gridID = PositionToGrid(piece.position);
            _grid[gridID.x, gridID.y] = STATIC_PIECE;
        }
    }

    private void OnDrawGizmos()
    {
        if (_grid == null)
        {
            InitializeGrid();
        }

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                switch (_grid[x, y])
                {
                    case 0:
                        Gizmos.color = Color.gray;
                        break;
                    case 1:
                        Gizmos.color = Color.green;
                        break;
                    case 2:
                        Gizmos.color = Color.yellow;
                        break;
                    case 3:
                        Gizmos.color = Color.red;
                        break;
                }

                Gizmos.DrawWireCube(_BottomLeftCorner + new Vector2(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f), new Vector2(cellSize, cellSize));
            }
        }
    }
}
