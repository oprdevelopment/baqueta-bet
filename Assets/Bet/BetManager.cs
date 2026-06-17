using System;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    [SerializeField] CurrencyManager currencyManager;
    [SerializeField] MatchManager gameSimulator;
    List<ComparisonBet> betList;
    void Start()
    {
        betList = new();
    }

    void OnCreatedMatch(MatchInfo matchInfo)
    {
        var newBet = new FoulComparisonBet(1.5f, this, 2.5f, ComparisonType.LessThan, matchInfo, currencyManager);
        newBet.PlaceBet(5);
        betList.Add(newBet);
    }

    void OnEnable()
    {
        MatchManager.CreatedMatch += OnCreatedMatch;
    }
    void OnDisable()
    {
        MatchManager.CreatedMatch -= OnCreatedMatch;
    }
}
