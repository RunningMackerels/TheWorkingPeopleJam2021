using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;

    private Vector2Int _direction = Vector2Int.down;

    public static GameState Instance => _instance;

    public Vector2Int DirectionGrid => _direction;
    public Vector2 Direction
    {
        get
        {
            return new Vector2(_direction.x, _direction.y);
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
