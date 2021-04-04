using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public enum Direction
    {
        Up = 1,
        Down = -1
    }
    
    public bool AutomaticSpawn = false;
    public float TimeBetweenSpawns = 0.5f;
    public Direction StartingDirection = Direction.Down;
}
