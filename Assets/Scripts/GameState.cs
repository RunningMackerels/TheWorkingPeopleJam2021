using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

public class GameState : MonoBehaviour
{
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

    [SerializeField]
    private Spawner spawner = null;

    public PlayArea PlayArea => playArea;

    public List<Tetrimo> InstancedTetrimos = new List<Tetrimo>();

    private int _currentTetrimoFalling = -1;

    private Stage _currentStage = Stage.Playing;
    public Stage CurrentStage
    {
        get => _currentStage;
        set
        {
            if (_currentStage != value)
            {
                _currentStage = value;

                //if we are changing to playing, then spawn a piece
                if (_currentStage == Stage.Playing)
                {
#if UNITY_EDITOR                
                    if(Config.AutomaticSpawn)
#endif                        
                    {
                        spawner.SpawnPiece();
                    }
                }
            }
        }
    }

    public Action<Stage> OnStageChanged;

    [SerializeField] 
    private GameConfig config = default;
    public GameConfig Config => config;
    
    public int Score { private set; get; } = 0;

    private float _pulse = 0f;
    private float _pulseTime;
    public float Pulse => _pulse;
    
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

        _pulse = Config.Pulse.y;
    }

    private void Start()
    {
        Score = 0;

        PlayArea.InitializeGrid();
        _direction = new Vector2Int(0, (int) config.StartingDirection);

#if UNITY_EDITOR
        if (Config.AutomaticSpawn)
#endif
        {
            spawner.SpawnPiece();
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
        MakeOneFall(null);
    }

    private void MakeOneFall(Tetrimo tetrimo)
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
            bool willRain = false;
            if (Direction.y < 0)
            {
                if (piece.Parts.Any(part => PlayArea.PositionToGrid(part.transform.position).y > aboveRow))
                {
                    willRain = true;
                }
            }
            else
            {
                if (piece.Parts.Any(part => PlayArea.PositionToGrid(part.transform.position).y < aboveRow))
                {
                    willRain = true;
                }
            }

            if (willRain)
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
        Score += Config.GetScore(numberOfLines);
    }

    public float GetCurrentBaseSpeed()
    {
        return Config.GetBaseSpeed(Score);
    }

    public void GameOver()
    {
        
    }
    
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            spawner.SpawnPiece();
        }
#endif

        Pulsating();
    }

    private void Pulsating()
    {
        if (_pulse == Config.Pulse.y)
        {
            var tmp = Config.Pulse.y;
            Config.Pulse.y = Config.Pulse.x;
            Config.Pulse.x = tmp;

            _pulseTime = 0;
        }

        _pulseTime += Time.deltaTime;
        _pulse = Mathf.SmoothStep(Config.Pulse.x, Config.Pulse.y, _pulseTime / Config.PulseTiming);
    }
}
