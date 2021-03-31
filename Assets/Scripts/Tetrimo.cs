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
    }

    private void CheckAndMoveSides()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            float distanceToColision = float.MaxValue;
            foreach (Transform part in parts)
            {
                float distance = PlayArea.GetDistanceToCollision(part.position, GameState.Instance.LeftDirectionGrid);

                if (distance < distanceToColision)
                {
                    distanceToColision = distance;
                }
            }

            if (distanceToColision > 0)
            {
                transform.Translate(GameState.Instance.LeftDirection * PlayArea.CellSize);
            }

            CalculateEndPosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            float distanceToColision = float.MaxValue;
            foreach (Transform part in parts)
            {
                float distance = PlayArea.GetDistanceToCollision(part.position, GameState.Instance.RightDirectionGrid);

                if (distance < distanceToColision)
                {
                    distanceToColision = distance;
                }
            }

            if (distanceToColision > 0)
            {
                transform.Translate(GameState.Instance.RightDirection * PlayArea.CellSize);
            }

            CalculateEndPosition();
        }
    }

    private void CalculateEndPosition()
    {
        _distanceToColision = float.MaxValue;

        foreach(Transform part in parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.position, GameState.Instance.DirectionGrid);

            Debug.LogWarning(part.name + " " + distance.ToString("N2"));

            if (distance < _distanceToColision)
            {
                _distanceToColision = distance;
            }
        }
    }
}
