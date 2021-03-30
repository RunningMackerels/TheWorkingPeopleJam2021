using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private PlayArea _playArea;

    [SerializeField]
    private List<Tetrimo> _TetrimosPrefabs = new List<Tetrimo>();

    private List<Tetrimo> _InstancedTetrimos = new List<Tetrimo>();

    [SerializeField]
    private Tetrimo _nextPiece = null;

    private void Awake()
    {
        PrepareNextPierce();
    }

    private void SpawnPiece()
    {
        Tetrimo spawnedPiece = Instantiate(_nextPiece, transform.position, transform.rotation, _playArea.transform);
        spawnedPiece.PlayArea = _playArea;

        _InstancedTetrimos.Add(spawnedPiece);

        PrepareNextPierce();
    }

    private void PrepareNextPierce()
    {
        int pieceIdx = Mathf.RoundToInt(Random.Range(0, _TetrimosPrefabs.Count));
        _nextPiece = _TetrimosPrefabs[pieceIdx];
    }

    private void ClearBoard()
    {
        for (int i = 0; i < _InstancedTetrimos.Count; i++)
        {
            Destroy(_InstancedTetrimos[i].gameObject);
        }
        _InstancedTetrimos.Clear();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnPiece();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ClearBoard();
        }
#endif
    }
}
