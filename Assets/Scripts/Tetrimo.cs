using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class Tetrimo : MonoBehaviour
{
    private enum State
    {
        Stopped,
        Moving,
    }

    [SerializeField]
    private TetrimoConfig _Config = null;

    private CompositeCollider2D _Collider = null;

    private State _State = State.Moving;

    private void Awake()
    {
        _Collider = GetComponent<CompositeCollider2D>();
    }

    private void Update()
    {
        if (_State == State.Stopped)
        {
            return;
        }

        RaycastHit2D[] results = new RaycastHit2D[1];
        float distance = Time.deltaTime * _Config.VerticalSpeed;

        int collisionsCount = _Collider.Cast(GameState.Instance.Direction, results, distance);

        if (collisionsCount == 0)
        {
            transform.Translate(GameState.Instance.Direction * distance);
        }
        else
        {
            _State = State.Stopped;
            //align to grid
        }
    }
}
