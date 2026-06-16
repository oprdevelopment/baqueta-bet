using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Team : ScriptableObject
{
    public List<String> players;
    
}

[Serializable]
public struct Foul
{
    public enum CardGiven
    {
        None,
        Yellow,
        Red
    }
    
    public CardGiven card;
    public string player;
}
[Serializable]
public class GameInfo
{
    public Team home, away;
    (int, int) score = (0, 0);
    public Foul[] fouls;
    public void Foul(string player)
    {
        
    }
}

public class GameSimulator : MonoBehaviour
{
    public static event Action Foul;
    public static event Action Card;
    public static event Action Goal;
    public static event Action GameEnded;
    public static event Action TestAction;
    public GameInfo gameInfo;
}
