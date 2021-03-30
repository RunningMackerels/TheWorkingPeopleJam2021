using UnityEngine;

public class PlayArea : MonoBehaviour
{
    private const int EMPTY = 0;
    private const int MOVING_PIECE = 1;
    private const int STATIC_PIECE = 2;
    private const int BORDER = 3;

    [SerializeField, Range(0f, 50f)]
    private int _Width = 20;

    [SerializeField, Range(0f, 50f)]
    private int _Height = 50;

    [SerializeField]
    private float _CellSize = 1f;

    /// <summary>
    /// 0 - empty
    /// 1 - moving piece
    /// 2 - static piece
    /// 3 - border
    /// </summary>
    private int[,] _grid = null;

    private Vector2 _BottomLeftCorner => transform.position - new Vector3(_CellSize * _Width * 0.5f, _CellSize * _Height * 0.5f, 0f);
    private Vector2 _TopRightCorner => transform.position + new Vector3(_CellSize * _Width * 0.5f, _CellSize * _Height * 0.5f, 0f);

    private Rect _Extentents => new Rect(_BottomLeftCorner, new Vector2(_Width * _CellSize, _Height * _CellSize));


    private void Awake()
    {
        InitializeGrid();
    }

    private void OnValidate()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _grid = new int[_Width, _Height];
        for (int x = 0; x < _Width; x++)
        {
            for (int y = 0; y < _Height; y++)
            {
                _grid[x, y] = EMPTY;
            }
        }
        for (int x = 0; x < _Width; x++)
        {
            _grid[x, 0] = 3;
            _grid[x, _Height - 1] = BORDER;
        }
        for (int y = 0; y < _Height; y++)
        {
            _grid[0, y] = 3;
            _grid[_Width - 1, y] = BORDER;
        }
    }

    public float GetDistanceToCollision(Vector2 origin, Vector2Int direction)
    {
        Vector2Int originGrid = PositionToGrid(origin);

        int gridDistance = DistanceToNextOccupiedGrid(originGrid, direction);

        float distanceToGridCenter = (origin - _BottomLeftCorner - Vector2.one * _CellSize * 0.5f - new Vector2(originGrid.x * _CellSize, originGrid.y * _CellSize)).magnitude;

        return distanceToGridCenter + gridDistance * _CellSize;
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
        Debug.Assert(position.x >= _BottomLeftCorner.x + _CellSize * 0.5f &&
                     position.y >= _BottomLeftCorner.y + _CellSize * 0.5f &&
                     position.x <= _TopRightCorner.x - _CellSize * 0.5f &&
                     position.y <= _TopRightCorner.y - _CellSize * 0.5f);

        Vector2 relativePosition = position - _BottomLeftCorner;

        return new Vector2Int((int)relativePosition.x / (int)_CellSize, (int)relativePosition.y / (int)_CellSize);
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

                Gizmos.DrawWireCube(_BottomLeftCorner + new Vector2(x * _CellSize + _CellSize * 0.5f, y * _CellSize + _CellSize * 0.5f), new Vector2(_CellSize, _CellSize));
            }
        }
    }
}
