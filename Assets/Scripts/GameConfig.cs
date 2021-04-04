using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Serializable]
    private class ScoreWeight
    {
        public int NumberOfLines;
        public int Score = 10;
    }

    [Serializable]
    private class DifficultyLevel
    {
        public int LowerLevel;
        public float Speed;
    }

    public enum Direction
    {
        Up = 1,
        Down = -1
    }
    
    public bool AutomaticSpawn = false;
    public float TimeBetweenSpawns = 0.5f;
    public Direction StartingDirection = Direction.Down;


    public Sprite[] TetrimoParts;
    public float PulseTiming = 2f;
    public Vector2 Pulse = new Vector2(90f, 100f);

    [SerializeField]
    private ScoreWeight[] scoreWeights;

    [SerializeField]
    private int scorePerLevel = 1000;

    [SerializeField]
    private List<DifficultyLevel> difficultyChart;

    private void OnEnable()
    {
        difficultyChart = difficultyChart.OrderByDescending(i => i.LowerLevel).ToList();
    }

    public int GetScore(int numberOfLines)
    {
        return scoreWeights.FirstOrDefault(s => s.NumberOfLines == numberOfLines).Score;
    }

    public float GetBaseSpeed(int score)
    {
        int level = score / scorePerLevel;
        Debug.LogWarning(level);
        return difficultyChart.FirstOrDefault(s => level >= s.LowerLevel).Speed;
    }
}
