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

    [SerializeField]
    private PlayArea playArea = null;

    public PlayArea PlayArea => playArea;

    public List<Tetrimo> InstancedTetrimos = new List<Tetrimo>();

    private int _currentTetrimoFalling = -1;

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

    public void ClearBoard()
    {
        for (int i = 0; i < InstancedTetrimos.Count; i++)
        {
            Destroy(InstancedTetrimos[i].gameObject);
        }
        InstancedTetrimos.Clear();
        playArea.InitializeGrid();
    }

    public void MakeItRain()
    {
        InstancedTetrimos.Sort();

        _currentTetrimoFalling = -1;
        MakeOneFall();

        //make them fall
    }

    private void MakeOneFall()
    {
        if (_currentTetrimoFalling >= 0)
        {
            InstancedTetrimos[_currentTetrimoFalling].OnStopped -= MakeOneFall;
        }

        _currentTetrimoFalling++;

        if (_currentTetrimoFalling == InstancedTetrimos.Count)
        {
            //no more to fall here
            return;
        }

        InstancedTetrimos[_currentTetrimoFalling].OnStopped += MakeOneFall;
        InstancedTetrimos[_currentTetrimoFalling].MakeItFall();
    }
}
