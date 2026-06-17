using System;
using System.Collections.Generic;
using UnityEngine;
public class GameSimulator : MonoBehaviour
{
    public event Action<MatchInfo> CreatedMatch;
    public List<MatchInfo> Matches{get; private set;}
    public event Action Tick;
    float elapsedTime = 0;

    void Start()
    {
        Matches = new();
        CreateNewMatch();
    }

    void CreateNewMatch()
    {
        MatchInfo newMatch = new(Team.GenerateRandomTeam(), Team.GenerateRandomTeam(), this);
        Matches.Add(newMatch);
        CreatedMatch?.Invoke(newMatch);
    }
    void Update()
    {
        if(elapsedTime >= 1)
        {
            elapsedTime = 0;
            Tick?.Invoke();
        }
        
        elapsedTime += Time.deltaTime;
    }
}
