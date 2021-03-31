using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Tetrimo Config", menuName = "ScriptableObjects/TetrimoConfig", order = 1)]
public class TetrimoConfig : ScriptableObject
{
    public float verticalSpeed = 5f;
    public float verticalBoost = 2f;
}
