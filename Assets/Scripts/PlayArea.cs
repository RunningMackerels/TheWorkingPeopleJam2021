using System.Collections.Generic;
using System.Linq;
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
    private Grid _grid = null;

    private Vector2 _BottomLeftCorner => transform.position - new Vector3(cellSize * width * 0.5f, cellSize * height * 0.5f, 0f);
    private Vector2 _TopRightCorner => transform.position + new Vector3(cellSize * width * 0.5f, cellSize * height * 0.5f, 0f);

    private Vector2 _HalfCellSize => new Vector2(cellSize * 0.5f, cellSize * 0.5f);
    private Rect _Extentents => new Rect(_BottomLeftCorner, new Vector2(width * cellSize, height * cellSize));

    public float CellSize => cellSize;

    private Dictionary<Vector2Int, TetrimoPart> _placedPieces = new Dictionary<Vector2Int, TetrimoPart>();

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
        _grid = new Grid(width, height);
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
        Vector2 directionFilter = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

        Vector2Int originGrid = PositionToGrid(origin);
        Vector2 originGridCenter = GridCenter(originGrid);

        int gridDistance = DistanceToNextOccupiedGrid(originGrid, direction);

        Vector2 originToCenterOrigin = origin - originGridCenter;
        float distanceToGridCenter = originToCenterOrigin.x * directionFilter.x + originToCenterOrigin.y * directionFilter.y;

        return gridDistance * cellSize + distanceToGridCenter;
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

    public bool CheckInterception(List<TetrimoPart> parts)
    {
        foreach(TetrimoPart part in parts)
        {
            Vector2Int gridID = PositionToGrid(part.transform.position);
            if (_grid[gridID.x, gridID.y] > EMPTY)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GridCenter(Vector2Int gridPosition)
    {
        return _BottomLeftCorner + new Vector2(gridPosition.x, gridPosition.y) * cellSize + Vector2.one * 0.5f * cellSize;
    }

    public Vector3 Behave(Vector3 origin)
    {
        Vector2Int originGrid = PositionToGrid(origin);
        Vector2 originGridCenter = GridCenter(originGrid);

        return originGridCenter;
    }

    public void PlacePieces(List<TetrimoPart> parts)
    {
        AssignType(parts, STATIC_PIECE);

        bool rowCleared = false;

        for (int y = 1; y < height - 1; y++)
        {
            IEnumerable<KeyValuePair<Vector2Int, TetrimoPart>> placePieceInLine = _placedPieces.Where(item => item.Key.y == y && _grid[item.Key.x, item.Key.y] == STATIC_PIECE);
            if (placePieceInLine.Count() == (width - 2))
            {
                foreach(KeyValuePair<Vector2Int, TetrimoPart> piece in placePieceInLine)
                {
                    piece.Value.Remove();
                    _grid[piece.Key.x, piece.Key.y] = EMPTY;
                }

                rowCleared = true;
            }
        }

        if (rowCleared)
        {
            GameState.Instance.CheckTetrimosIntegrity();
            //GameState.Instance.DropOne(GameState.Instance.Direction.x);
        }
    }

    public void RemoveStatic(List<TetrimoPart> parts)
    {
        AssignType(parts, EMPTY);
    }

    private void AssignType(List<TetrimoPart> parts, int type)
    {
        foreach (TetrimoPart part in parts)
        {
            Vector2Int gridID = PositionToGrid(part.transform.position);
            _grid[gridID.x, gridID.y] = type;
            _placedPieces[gridID] = part;
        }
    }

    public HashSet<Tetrimo> GetAdjacentPieces(List<TetrimoPart> parts, Vector2Int directionNormalized)
    {
        HashSet<Tetrimo> adjacent = new HashSet<Tetrimo>();

        foreach(TetrimoPart part in parts)
        {
            Vector2Int gridPos = PositionToGrid(part.transform.position);

            while (_grid[gridPos] != BORDER)
            {
                gridPos += directionNormalized;

                switch (_grid[gridPos])
                {
                    case EMPTY:
                        continue;
                    case STATIC_PIECE:
                        if (_placedPieces[gridPos].ParentTetrimo != part.ParentTetrimo)
                        {
                            adjacent.Add(_placedPieces[gridPos].ParentTetrimo);
                        }
                        continue;
                }
            }
        }

        return adjacent;
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

        Gizmos.color = Color.white;
        Gizmos.DrawLine(_BottomLeftCorner, _TopRightCorner);
    }
}
