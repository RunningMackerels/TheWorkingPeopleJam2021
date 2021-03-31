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
