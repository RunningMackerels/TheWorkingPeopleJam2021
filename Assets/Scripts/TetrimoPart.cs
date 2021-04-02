using System;
using UnityEngine;

/// <summary>
/// Each square of a tetrimo
/// </summary>
public class TetrimoPart : MonoBehaviour
{
    [HideInInspector]
    public Tetrimo ParentTetrimo;

    public void Remove()
    {
        ParentTetrimo.RemovePart(this);
        Destroy(gameObject);
    }
}
