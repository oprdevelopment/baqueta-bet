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
        int matchPerDay = 23 / Objective.MaxDays;
        for(int i = 0; i <= Objective.MaxDays; i++)
        {
            for(int j = 0; i < matchPerDay; i++)
            {
                CreateNewMatch(j);
            }
        }
    }

    void CreateNewMatch(int day)
    {
        MatchInfo newMatch = new(Team.GenerateRandomTeam(), Team.GenerateRandomTeam(), this, day);
        Matches.Add(newMatch);
        CreatedMatch?.Invoke(newMatch);
    }
}
