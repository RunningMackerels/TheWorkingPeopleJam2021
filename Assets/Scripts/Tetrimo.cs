using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

public class Tetrimo : MonoBehaviour
{
    private enum State
    {
        Stopped,
        Moving,
    }

    
    const int NSubdivisions = 3;
        
    [SerializeField]
    private TetrimoConfig config = null;

    private BoxCollider2D[] _colliders = null;

    private State _state = State.Moving;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<BoxCollider2D>();
    }

    private void Update()
    {
        if (_state == State.Stopped)
        {
            return;
        }
        
        MoveOrStop();
    }

    private void MoveOrStop()
    {
        bool hit = false;
        float vMov = -Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f);
        float distance = vMov > 0 ? 
            Time.deltaTime * config.verticalSpeed * config.verticalBoost * vMov : 
            Time.deltaTime * config.verticalSpeed;

        int tries = 0;
        do
        {
            hit = CanItMoveDown(distance);
            if (!hit)
            {
                transform.Translate(GameState.Instance.Direction * distance);
            }
            else
            {
                distance = distance * 0.5f;
            }
        } while (hit && tries++ < NSubdivisions);
        
        if (hit)
        {
            _state = State.Stopped;
            RoundToGrid();
        }
    }

    private void RoundToGrid()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Floor(position.y + 0.5f);
        transform.position = position;
    }

    private bool CanItMoveDown(float distance)
    {
        bool hit = false;
        foreach (Collider2D col in _colliders)
        {
            RaycastHit2D[] results = new RaycastHit2D[1];

            
            int collisionsCount = col.Cast(GameState.Instance.Direction, results, distance);
            if (collisionsCount > 0 && results[0].normal.x != 0)
            {
                collisionsCount = 0;
            }
            Debug.DrawRay(col.transform.position, GameState.Instance.Direction, Color.blue);

            hit |= collisionsCount > 0;
        }

        return hit;
    }
}
