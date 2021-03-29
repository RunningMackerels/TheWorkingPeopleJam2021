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

    private BoxCollider2D[] _colliders = null;

    private State _State = State.Moving;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<BoxCollider2D>();
    }

    private void Update()
    {
        if (_State == State.Stopped)
        {
            return;
        }

        bool hit = false;
        float distance = Time.deltaTime * _Config.VerticalSpeed;

        foreach (Collider2D col in _colliders)
        {
            RaycastHit2D[] results = new RaycastHit2D[1];
            
            int collisionsCount = col.Cast(GameState.Instance.Direction, results, distance);

            Debug.DrawRay(col.transform.position, GameState.Instance.Direction, Color.blue);

            hit |= collisionsCount > 0;
        }

        if (!hit)
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
