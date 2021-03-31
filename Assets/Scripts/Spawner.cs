using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private PlayArea _playArea;

    [SerializeField]
    private List<Tetrimo> tetrimosPrefabs = new List<Tetrimo>();

    private List<Tetrimo> _instancedTetrimos = new List<Tetrimo>();

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

        _instancedTetrimos.Add(spawnedPiece);

        PrepareNextPierce();
    }

    private void PrepareNextPierce()
    {
        int pieceIdx = Mathf.RoundToInt(Random.Range(0, tetrimosPrefabs.Count));
        _nextPiece = tetrimosPrefabs[pieceIdx];
    }

    private void ClearBoard()
    {
        for (int i = 0; i < _instancedTetrimos.Count; i++)
        {
            Destroy(_instancedTetrimos[i].gameObject);
        }
        _instancedTetrimos.Clear();
        _playArea.InitializeGrid();
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
