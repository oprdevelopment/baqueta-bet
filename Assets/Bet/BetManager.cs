using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
}
