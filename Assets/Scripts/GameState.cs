using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _Instance;

    private Vector2 _Direction = Vector2.down;

    public static GameState Instance => _Instance;

    public Vector2 Direction => _Direction;

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
