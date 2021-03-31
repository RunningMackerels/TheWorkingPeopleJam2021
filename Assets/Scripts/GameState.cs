using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;

    private Vector2Int _direction = Vector2Int.down;

    private Vector2Int _leftDirection = Vector2Int.left;

    private Vector2Int _rightDirection = Vector2Int.right;

    public static GameState Instance => _instance;

    public Vector2Int DirectionGrid => _direction;
    public Vector2 Direction
    {
        get
        {
            return new Vector2(_direction.x, _direction.y);
        }
    }

    public Vector2Int LeftDirectionGrid => _leftDirection;
    public Vector2 LeftDirection
    {
        get
        {
            return new Vector2(_leftDirection.x, _leftDirection.y);
        }
    }

    public Vector2Int RightDirectionGrid => _rightDirection;
    public Vector2 RightDirection
    {
        get
        {
            return new Vector2(_rightDirection.x, _rightDirection.y);
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
