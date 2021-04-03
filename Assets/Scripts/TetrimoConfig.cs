using UnityEngine;

[CreateAssetMenu(fileName = "Tetrimo Config", menuName = "ScriptableObjects/TetrimoConfig", order = 1)]
public class TetrimoConfig : ScriptableObject
{
    public float VerticalSpeed = 5f;
    public float VerticalBoost = 2f;
    public float DropMultiplier = 10f;
}
