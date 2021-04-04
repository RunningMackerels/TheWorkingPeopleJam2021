using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private PlayArea _playArea;

    [SerializeField]
    private List<Tetrimo> tetrimosPrefabs = new List<Tetrimo>();

    [SerializeField]
    private Tetrimo _nextPiece = null;

    [SerializeField]
    private Transform nextPieceUIRoot;

    private int _pieceID = 0;

    private Tetrimo _currentPiece = null;

    private void Awake()
    {
        PrepareNextPierce();
    }

    public void SpawnPiece()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x * -1f * GameState.Instance.Direction.x,
                                            transform.position.y * -1f * GameState.Instance.Direction.y,
                                            0f);
        Tetrimo spawnedPiece = _nextPiece;
        spawnedPiece.transform.position = spawnPosition;
        spawnedPiece.transform.rotation = transform.rotation;
        spawnedPiece.transform.parent = _playArea.transform;
        spawnedPiece.Enable();

        if (GameState.Instance.PlayArea.CheckInterception(spawnedPiece.Parts))
        {
            GameState.Instance.GameOver();
            return;
        }

        GameState.Instance.InstancedTetrimos.Add(spawnedPiece);

        _currentPiece = spawnedPiece;
        _currentPiece.OnStopped += HandleCurrentPieceStopped;

        _pieceID++;
        PrepareNextPierce();
    }

    private void HandleCurrentPieceStopped(Tetrimo tetrimo)
    {
        _currentPiece.OnStopped -= HandleCurrentPieceStopped;
        _currentPiece = null;

        if (GameState.Instance.CurrentStage == GameState.Stage.Playing)
        {
#if UNITY_EDITOR
            if (GameState.Instance.Config.AutomaticSpawn)
#endif
            {
                SpawnPiece();
            }
        }
    }

    private void PrepareNextPierce()
    {
        int pieceIdx = Mathf.RoundToInt(Random.Range(0, tetrimosPrefabs.Count));
        _nextPiece = Instantiate(tetrimosPrefabs[pieceIdx], nextPieceUIRoot, false);

        _nextPiece.name = _pieceID.ToString() + "_" + _nextPiece.name;
        _nextPiece.Disable();
    }
}
