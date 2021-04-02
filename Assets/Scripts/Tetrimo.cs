using System;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimo : MonoBehaviour, IComparable<Tetrimo>
{
    private enum State
    {
        Stopped,
        Interactable,
        Falling,
    }

    [SerializeField]
    private TetrimoConfig config = null;

    [SerializeField]
    private List<TetrimoPart> parts;

    private State _state = State.Interactable;

    float _distanceToColision = float.MaxValue;

    public PlayArea PlayArea { get; set; }

    public Action OnStopped;

    private void Start()
    {
        foreach (TetrimoPart part in parts)
        {
            part.ParentTetrimo = this;
        }
        CalculateEndPosition();
    }

    private void Update()
    {
        if (_state == State.Stopped)
        {
            return;
        }

        float speedMultiplier = 1f;

        if (_state == State.Interactable)
        {
            if (!CheckAndMoveSides())
            {
                CheckAndRotate();
            }

            speedMultiplier = -Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f) * config.VerticalBoost;
        }

        float distance = Time.deltaTime * (config.VerticalSpeed + speedMultiplier);

        
        if (distance > _distanceToColision)
        {
            distance = _distanceToColision;
            _distanceToColision = 0;

            _state = State.Stopped;
            OnStopped?.Invoke();
            PlayArea.PlacePieces(parts);
        }
        else
        {
            _distanceToColision -= distance;
        }

        transform.Translate(GameState.Instance.Direction * distance, Space.World);

        //it is triggering an assert when falling, and it is not really necessary
        //if (_state == State.Stopped)
        //{
        //    transform.position = PlayArea.Behave(transform.position);
        //}
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(90f);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(-90f);
        }
    }

    private void MoveSideways(Vector2Int directionGrid, Vector2 direction)
    {
        float distanceToColision = float.MaxValue;
        foreach (TetrimoPart part in parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.transform.position, directionGrid);

            if (distance < distanceToColision)
            {
                distanceToColision = distance;
            }
        }

        if (distanceToColision > 0)
        {
            transform.Translate(direction * PlayArea.CellSize, Space.World);
        }

        CalculateEndPosition();
    }

    private void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0f, 0f, angle));

        if (PlayArea.CheckInterception(parts))
        {
            transform.Rotate(new Vector3(0f, 0f, -angle));
        }

        CalculateEndPosition();
    }

    private void CalculateEndPosition()
    {
        _distanceToColision = float.MaxValue;

        foreach(TetrimoPart part in parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.transform.position, GameState.Instance.DirectionGrid);

            if (distance < _distanceToColision)
            {
                _distanceToColision = distance;
            }
        }
    }

    public void RemovePart(TetrimoPart tetrimoPart)
    {
        parts.Remove(tetrimoPart);

        if (parts.Count == 0)
        {
            GameState.Instance.InstancedTetrimos.Remove(this);
            Destroy(gameObject);
        }

        //TODO add logic to split the tetrimo
    }

    public void MakeItFall()
    {
        PlayArea.RemoveStatic(parts);

        CalculateEndPosition();

        _state = State.Falling;
    }

    public int CompareTo(Tetrimo other)
    {
        HashSet<Tetrimo> adjacent = GameState.Instance.PlayArea.GetAdjacentPieces(parts, GameState.Instance.DirectionGrid);

        if (adjacent.Contains(other))
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
