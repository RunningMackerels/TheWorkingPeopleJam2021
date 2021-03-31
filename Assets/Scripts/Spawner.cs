using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [FormerlySerializedAs("_TetrimosPrefabs")] [SerializeField]
    private List<Tetrimo> _tetrimosPrefabs = new List<Tetrimo>();

    private List<Tetrimo> _instancedTetrimos = new List<Tetrimo>();

    [ContextMenu("Spawn Piece")]
    private void SpawnPiece()
    {
        int pieceIdx = Mathf.RoundToInt(Random.Range(0, _tetrimosPrefabs.Count));

        _instancedTetrimos.Add(Instantiate(_tetrimosPrefabs[pieceIdx], transform, false));
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
