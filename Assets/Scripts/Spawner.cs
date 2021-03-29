using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private List<Tetrimo> _TetrimosPrefabs = new List<Tetrimo>();

    private List<Tetrimo> _InstancedTetrimos = new List<Tetrimo>();

    [ContextMenu("Spawn Piece")]
    private void SpawnPiece()
    {
        int pieceIdx = Mathf.RoundToInt(Random.Range(0, _TetrimosPrefabs.Count));

        _InstancedTetrimos.Add(Instantiate(_TetrimosPrefabs[pieceIdx], transform, false));
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnPiece();
        }
#endif
    }
}
