using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _Instance;

    private Vector2Int _Direction = Vector2Int.down;

    public static GameState Instance => _Instance;

    public Vector2Int DirectionGrid => _Direction;
    public Vector2 Direction
    {
        get
        {
            return new Vector2(_Direction.x, _Direction.y);
        }
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _Instance = this;
        }
    }
}
