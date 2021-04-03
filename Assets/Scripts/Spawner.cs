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
        Tetrimo spawnedPiece = Instantiate(_nextPiece, spawnPosition, transform.rotation, _playArea.transform);
        spawnedPiece.name = _pieceID.ToString() + "_" + _nextPiece.name;

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
        _nextPiece = tetrimosPrefabs[pieceIdx];
    }
}
