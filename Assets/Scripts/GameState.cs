using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [Serializable]
    private class ScoreWeight
    {
        public int NumberOfLines;
        public int Score = 10;
    }

    public enum Stage
    {
        Playing,
        Reversing,
        Size
    }
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

    public Stage CurrentStage { get; set; } = Stage.Playing;


    [SerializeField]
    private ScoreWeight[] scoreWeights;

    private int _score = 0;

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

        _score = 0;
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

    public void CheckTetrimosIntegrity()
    {
        int initialNumberOfTetrimos = InstancedTetrimos.Count;

        for(int i = 0; i < initialNumberOfTetrimos; i++)
        {
            InstancedTetrimos[i].CheckIntegrity();
        }
    }

    public void MakeItRain()
    {
        InstancedTetrimos.Sort();

        _currentTetrimoFalling = -1;
        MakeOneFall();
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

    public void ControlledRain(int aboveRow)
    {
        List<Tetrimo> toMove = new List<Tetrimo>();

        foreach(Tetrimo piece in InstancedTetrimos)
        {
            //TODO need to be tweaked for different orientation
            if (piece.Parts.Any(part => PlayArea.PositionToGrid(part.transform.position).y > aboveRow))
            {
                PlayArea.RemoveStatic(piece.Parts);
                piece.transform.Translate(Direction * playArea.CellSize, Space.World);
                playArea.AssignType(piece.Parts, PlayArea.STATIC_PIECE);
            }
        }
    }

    public void ReverseDirection()
    {
        _direction *= Vector2Int.down;
    }

    public void AddScore(int numberOfLines)
    {
        _score += scoreWeights.FirstOrDefault(s => s.NumberOfLines == numberOfLines).Score;
    }

}
