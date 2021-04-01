using System;
using UnityEngine;

public class Tetrimo : MonoBehaviour
{
    private enum State
    {
        Stopped,
        Moving,
    }

    [SerializeField]
    private TetrimoConfig config = null;

    [SerializeField]
    private Transform[] parts;

    private State _state = State.Moving;

    float _distanceToColision = float.MaxValue;

    public PlayArea PlayArea { get; set; }

    private void Start()
    {
        CalculateEndPosition();
    }

    private void Update()
    {
        if (_state == State.Stopped)
        {
            return;
        }

        if (!CheckAndMoveSides())
        {
            CheckAndRotate();
        }

        float speedMultiplier = -Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f) * config.VerticalBoost;        

        float distance = Time.deltaTime * (config.VerticalSpeed + speedMultiplier);

        
        if (distance > _distanceToColision)
        {
            distance = _distanceToColision;
            _distanceToColision = 0;

            _state = State.Stopped;
            PlayArea.MarkStaticPieces(parts);
        }
        else
        {
            _distanceToColision -= distance;
        }

        transform.Translate(GameState.Instance.Direction * distance, Space.World);

        if (_state == State.Stopped)
        {
            transform.position = PlayArea.Behave(transform.position);
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
        foreach (Transform part in parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.position, directionGrid);

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

        foreach(Transform part in parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.position, GameState.Instance.DirectionGrid);

            if (distance < _distanceToColision)
            {
                _distanceToColision = distance;
            }
        }
    }
}
