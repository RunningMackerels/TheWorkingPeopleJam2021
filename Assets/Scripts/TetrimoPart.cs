using System;
using UnityEngine;

/// <summary>
/// Each square of a tetrimo
/// </summary>
public class TetrimoPart : MonoBehaviour
{
    public enum PartType
    {
        TopCap,
        BottomCap,
        LeftCap,
        RightCap,
        StraightHorizontal,
        StraightVertical,
        TopLeftCorner,
        TopRightCorner,
        BottomLeftCorner,
        BottomRightCorner,
        Single,
        None,
        Size
    }
    
    [HideInInspector]
    public Tetrimo ParentTetrimo;

    [SerializeField] 
    private PartType type = PartType.None;

    private SpriteRenderer _sr;
    private Vector3 _colorInHSV = Vector3.one;
    public void Setup()
    {
        _sr = GetComponent<SpriteRenderer>();
        Color.RGBToHSV(_sr.color, out _colorInHSV.x, out _colorInHSV.y, out _colorInHSV.z);
        Debug.Log(_colorInHSV);
#if UNITY_EDITOR
        if (type == PartType.None)
        {
            return;
        }
#endif
        _sr.sprite = GameState.Instance.Config.TetrimoParts[(int) type];
        _sr.color = ParentTetrimo.BaseColor;
        Color.RGBToHSV(_sr.color, out _colorInHSV.x, out _colorInHSV.y, out _colorInHSV.z);
    }

    public void Remove()
    {
        ParentTetrimo.RemovePart(this);
        Destroy(gameObject);
    }

    private void Update()
    {
        _colorInHSV.z = GameState.Instance.Pulse * (1.0f / 100.0f);
        _sr.color = Color.HSVToRGB(_colorInHSV.x, _colorInHSV.y, _colorInHSV.z);
    }
}
