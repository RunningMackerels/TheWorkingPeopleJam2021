using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tetrimo : MonoBehaviour
{
    private enum State
    {
        Stopped,
        Moving,
    }

    [SerializeField]
    private TetrimoConfig _Config = null;

    [SerializeField]
    private Transform[] _parts;

    private State _State = State.Moving;

    float _distanceToColision = float.MaxValue;

    public PlayArea PlayArea { get; set; }

    private void Start()
    {
        CalculateEndPosition();
    }



    private void Update()
    {
        if (_State == State.Stopped)
        {
            return;
        }

        float distance = Time.deltaTime * _Config.VerticalSpeed;

        if (distance > _distanceToColision)
        {
            distance = _distanceToColision;
            _distanceToColision = 0;

            _State = State.Stopped;
            PlayArea.MarkStaticPieces(_parts);
        }
        else
        {
            _distanceToColision -= distance;
        }

        transform.Translate(GameState.Instance.Direction * distance);
    }

    private void CalculateEndPosition()
    {
        _distanceToColision = float.MaxValue;

        foreach(Transform part in _parts)
        {
            float distance = PlayArea.GetDistanceToCollision(part.position, GameState.Instance.DirectionGrid);

            if (distance < _distanceToColision)
            {
                _distanceToColision = distance;
            }
        }
    }
}
