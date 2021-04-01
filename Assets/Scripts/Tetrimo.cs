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

        CheckAndMoveSides();

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

        transform.Translate(GameState.Instance.Direction * distance);

        if (_state == State.Stopped)
        {
            transform.position = PlayArea.Behave(transform.position);
        }
    }

    private void CheckAndMoveSides()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSideways(GameState.Instance.LeftDirectionGrid, GameState.Instance.LeftDirection);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSideways(GameState.Instance.RightDirectionGrid, GameState.Instance.RightDirection);
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

        Debug.LogError(distanceToColision.ToString("N2"));

        if (distanceToColision > 0)
        {
            transform.Translate(direction * PlayArea.CellSize);
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

        Debug.LogWarning(_distanceToColision.ToString("N2"));
    }
}
