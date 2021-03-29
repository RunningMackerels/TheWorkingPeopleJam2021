using UnityEngine;

public class PlayArea : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)]
    private int _Width = 20;

    [SerializeField, Range(0f, 50f)]
    private int _Height = 50;

    [SerializeField]
    private float _CellSize = 1f;

    [SerializeField]
    private Collider2D _TopCollider = null;

    [SerializeField]
    private Collider2D _BottomCollider = null;

    [SerializeField]
    private Collider2D _LeftCollider = null;

    [SerializeField]
    private Collider2D _RightCollider = null;

    private Vector2 _BottomLeftCorner => transform.position - new Vector3(_CellSize * _Width * 0.5f, _CellSize * _Height * 0.5f, 0f);

    private Rect _Extentents => new Rect(_BottomLeftCorner, new Vector2(_Width * _CellSize, _Height * _CellSize));


    private void Awake()
    {
        UpdateColliders();
    }

    private void OnValidate()
    {
        UpdateColliders();
    }

    private void UpdateColliders()
    {
        _TopCollider.transform.position = transform.position + new Vector3(0f, _Extentents.size.y * 0.5f, 0f);
        _TopCollider.transform.localScale = new Vector3(_Width * _CellSize, 1f, 1f);

        _BottomCollider.transform.position = transform.position - new Vector3(0f, _Extentents.size.y * 0.5f, 0f);
        _BottomCollider.transform.localScale = new Vector3(_Width * _CellSize, 1f, 1f);

        _LeftCollider.transform.position = transform.position - new Vector3(_Extentents.size.x * 0.5f, 0f, 0f);
        _LeftCollider.transform.localScale = new Vector3(1f, _Height * _CellSize, 1f);

        _RightCollider.transform.position = transform.position + new Vector3(_Extentents.size.x * 0.5f, 0f, 0f);
        _RightCollider.transform.localScale = new Vector3(1f, _Height * _CellSize, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for(int x = 0; x < _Width; x++)
        {
            for (int y = 0; y < _Height; y++)
            {
                Gizmos.DrawWireCube(_BottomLeftCorner + new Vector2(x * _CellSize + _CellSize * 0.5f, y * _CellSize + _CellSize * 0.5f), new Vector2(_CellSize, _CellSize));
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_Extentents.center, _Extentents.size);
    }
}
