using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tetrimo : MonoBehaviour
{
    private enum State
    {
        Stopped,
        Interactable,
        Falling,
        Dropping,
        Reverting,
        Size
    }

    [Flags]
    private enum Side
    {
        None                   = 0,
        Top                    = 1 << 0,
        Bottom                 = 1 << 1,
        Left                   = 1 << 2,
        Right                  = 1 << 3,
        BottomLeft             = Top | Right,
        BottomCap              = Top,
        BottomRight            = Top | Left,
        LeftCap                = Right,
        RightCap               = Left,
        StraightVertical       = Top | Bottom,
        StraightHorizontalLine = Left | Right | Top,
        StraightHorizontal     = Right | Left,
        TopCap                 = Bottom,
        TopLeft                = Bottom | Right,
        TopRight               = Bottom | Left,
        All                    = Top | Bottom | Left | Right
    }
    
    
    [SerializeField]
    private TetrimoConfig config = null;

    [SerializeField] 
    private Color baseColor = default;

    public Color BaseColor => baseColor;

    public List<TetrimoPart> Parts;

    private State _state = State.Interactable;

    float _distanceToColision = float.MaxValue;

    public PlayArea PlayArea => GameState.Instance.PlayArea;
    public bool IsStopped => _state == State.Stopped;

    public Action<Tetrimo> OnStopped;
    public Action<Tetrimo> OnMoved;

    private void Start()
    {
        SpriteItUp();
        foreach (TetrimoPart part in Parts)
        {
            part.ParentTetrimo = this;
            part.Setup();
        }
    }

    private void OnEnable()
    {
        OnStopped += SoundPlayer.Instance.PlayStopped;
        OnMoved += SoundPlayer.Instance.PlayHorizontalMove;
    }

    private void OnDisable()
    {
        OnStopped -= SoundPlayer.Instance.PlayStopped;
        OnMoved -= SoundPlayer.Instance.PlayHorizontalMove;
    }

    private void Update()
    {
        if (_state == State.Stopped)
        {
            return;
        }

        float speedMultiplier = 1f;

        switch(_state)
        {
            case State.Interactable:

                if (!CheckAndMoveSides())
                {
                    CheckAndRotate();
                }
                CheckAndDrop();
                speedMultiplier = -Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f) * config.VerticalBoost;
                break;

            case State.Dropping:
                speedMultiplier = config.DropMultiplier;
                break;
            
            case State.Reverting:
                speedMultiplier = config.DropMultiplier * 2.0f;
                break;
        }

        float distance = Time.deltaTime * (GameState.Instance.GetCurrentBaseSpeed() + speedMultiplier * config.VerticalBoost);

        int rowsCleared = 0;

        if (distance > _distanceToColision)
        {
            distance = _distanceToColision;
            _distanceToColision = 0;

            _state = State.Stopped;
        }
        else
        {
            _distanceToColision -= distance;
        }

        transform.Translate(GameState.Instance.Direction * distance, Space.World);

        if (_state == State.Stopped)
        {
            rowsCleared = PlayArea.PlacePieces(Parts);
            OnStopped?.Invoke(this);
        }
        
        if (rowsCleared == 0)
        {
            return;
        }

        if (GameState.Instance.CanFlip && UnityEngine.Random.value < GameState.Instance.Config.GetFlipProbability(rowsCleared))
        {
            PlayArea.FlipIt();
        }
    }

    private bool CheckAndMoveSides()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSideways(GameState.Instance.LeftDirectionGrid, GameState.Instance.LeftDirection);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSideways(GameState.Instance.RightDirectionGrid, GameState.Instance.RightDirection);
            return true;
        }

        return false;
    }

    private void CheckAndRotate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate(-90f);
        }
    }


    private void CheckAndDrop()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _state = State.Dropping;
        }
    }

    private void MoveSideways(Vector2Int directionGrid, Vector2 direction)
    {
        float distanceToColision = float.MaxValue;
        foreach (TetrimoPart part in Parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.transform.position, directionGrid);

            if (distance < distanceToColision)
            {
                distanceToColision = distance;
            }
        }

        //remove floating point errors
        distanceToColision = Mathf.Round(distanceToColision);

        if (distanceToColision > 0)
        {
            transform.Translate(direction * PlayArea.CellSize, Space.World);
            OnMoved?.Invoke(this);
        }

        CalculateEndPosition();
    }

    private void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0f, 0f, angle));

        if (PlayArea.CheckInterception(Parts))
        {
            transform.Rotate(new Vector3(0f, 0f, -angle));
        }
        else
        {
            OnMoved?.Invoke(this);
        }

        CalculateEndPosition();
    }

    public void CalculateEndPosition()
    {
        _distanceToColision = float.MaxValue;

        foreach(TetrimoPart part in Parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.transform.position, GameState.Instance.DirectionGrid);

            if (distance < _distanceToColision)
            {
                _distanceToColision = distance;
            }
        }

        _distanceToColision = _distanceToColision >= 0 ? _distanceToColision : 0;
    }

    public void RemovePart(TetrimoPart tetrimoPart)
    {
        Parts.Remove(tetrimoPart);
        SpriteItUp();
        
        if (Parts.Count == 0)
        {
            GameState.Instance.InstancedTetrimos.Remove(this);
            Destroy(gameObject);
        }
    }

    private void SpriteItUp()
    {
        foreach (TetrimoPart part in Parts)
        {
            bool top = false, bottom = false, left = false, right = false;
            foreach (TetrimoPart other in Parts)
            {
                if (part == other)
                {
                    continue;
                }
                
                
                
                var diff = part.transform.localPosition - other.transform.localPosition;
                if (diff.magnitude > 1)
                {
                    continue;
                }
                top |= diff.y < 0;
                bottom |= diff.y > 0;
                left |= diff.x > 0;
                right |= diff.x < 0;
            }

            Side result = Side.None;
            if (top)
            {
                result |= Side.Top;
            }

            if (bottom)
            {
                result |= Side.Bottom;
            }

            if (left)
            {
                result |= Side.Left;    
            }

            if (right)
            {
                result |= Side.Right;
            }

            switch (result)
            {
                case Side.None:
                    part.Type = TetrimoPart.PartType.Single;
                    break;
                case Side.BottomLeft:
                    part.Type = TetrimoPart.PartType.BottomLeftCorner;
                    break;
                case Side.BottomCap:
                    part.Type = TetrimoPart.PartType.BottomCap;
                    break;
                case Side.BottomRight:
                    part.Type = TetrimoPart.PartType.BottomRightCorner;
                    break;
                case Side.LeftCap:
                    part.Type = TetrimoPart.PartType.LeftCap;
                    break;
                case Side.RightCap:
                    part.Type = TetrimoPart.PartType.RightCap;
                    break;
                case Side.StraightVertical:
                    part.Type = TetrimoPart.PartType.StraightVertical;
                    break;
                case Side.StraightHorizontalLine:
                    part.Type = TetrimoPart.PartType.StraightHorizontalLine;
                    break;
                case Side.StraightHorizontal:
                    part.Type = TetrimoPart.PartType.StraightHorizontal;
                    break;
                case Side.TopCap:
                    part.Type = TetrimoPart.PartType.TopCap;
                    break;
                case Side.TopLeft:
                    part.Type = TetrimoPart.PartType.TopLeftCorner;
                    break;
                case Side.TopRight:
                    part.Type = TetrimoPart.PartType.TopRightCorner;
                    break;
                default:
                    Debug.Log(part.ParentTetrimo.gameObject.name + " | " + part.gameObject.name + " | " + result);
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }
    }

    public void CheckIntegrity()
    {
        HashSet<int> verticalPiecesIDs = new HashSet<int>();

        //check if they have holes
        foreach (TetrimoPart part in Parts)
        {
            verticalPiecesIDs.Add(PlayArea.PositionToGrid(part.transform.position).y);
        }

        List<int> verticalPiecesIDsList = verticalPiecesIDs.ToList();
        verticalPiecesIDsList.Sort();

        bool doSplit = false;

        List<TetrimoPart> newPieceParts = new List<TetrimoPart>();

        for(int i = 1; i < verticalPiecesIDsList.Count; i++)
        {
            if (verticalPiecesIDsList[i] != verticalPiecesIDsList[i-1] + 1)
            {
                doSplit = true;
            }

            if (doSplit)
            {
                List<TetrimoPart> toSwap = Parts.Where(part => GameState.Instance.PlayArea.PositionToGrid(part.transform.position).y == verticalPiecesIDsList[i]).ToList();
                toSwap.ForEach(part => Parts.Remove(part));
                newPieceParts.AddRange(toSwap);
            }
        }

        if (!doSplit)
        {
            return;
        }

        GameObject newGOPiece = new GameObject(name + "_part2");
        Transform newTransformPiece = newGOPiece.GetComponent<Transform>();
        newTransformPiece.position = transform.position;
        Tetrimo newPiece = newGOPiece.AddComponent<Tetrimo>();
        newPiece.baseColor = baseColor;
        newPiece.Parts = newPieceParts;
        newPiece.config = config;
        newPiece._state = State.Stopped;
        newPieceParts.ForEach(part => part.transform.parent = newTransformPiece);
        newTransformPiece.parent = transform.parent;

        GameState.Instance.InstancedTetrimos.Add(newPiece);

        name += "_part1";
    }

    public void MakeItFall()
    {
        PlayArea.RemoveStatic(Parts);

        CalculateEndPosition();

        _state = State.Falling;
    }

    public void Revert()
    {
        CalculateEndPosition();
        _state = State.Reverting;
    }

    public void Disable()
    {
        _state = State.Stopped;
    }

    public void Enable()
    {
        _state = State.Interactable;
        CalculateEndPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, GameState.Instance.Direction.y * _distanceToColision, 0f));
    }
}

public class TetrimoComparer : IComparer<Tetrimo>
{
    public int Compare(Tetrimo a, Tetrimo b)
    {
        if (a == b)
        {
            return 0;
        }

        List<Tetrimo> adjacent = GameState.Instance.PlayArea.GetAdjacentPieces(a.Parts, new Vector2Int(0, -1));

        if (adjacent.Contains(b))
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}