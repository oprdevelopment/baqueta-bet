using System;
using System.Collections.Generic;
using UnityEngine;
public class MatchManager : MonoBehaviour
{
    public static event Action<MatchInfo> CreatedMatch;
    public List<MatchInfo> Matches{get; private set;}
    float elapsedTime = 0;

    void Awake()
    {
        Matches = new();
        for(int i = 0; i < 20; i++)
        {
            CreateNewMatch();
        }
    }

    void CreateNewMatch()
    {
        MatchInfo newMatch = new(Team.GenerateRandomTeam(), Team.GenerateRandomTeam(), this);
        Matches.Add(newMatch);
        CreatedMatch?.Invoke(newMatch);
    }
}
