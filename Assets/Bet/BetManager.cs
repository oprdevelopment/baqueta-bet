using System;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    [SerializeField] CurrencyManager currencyManager;
    List<ComparisonBet> betList;
    public float timeElapsed;
    void Start()
    {
        betList = new();
        var newBet = new FoulComparisonBet(2, this, 4, ComparisonType.GreaterThan);
        newBet.PlaceBet(20, currencyManager);
        betList.Add(newBet);
    }

    void Update()
    {
        if(timeElapsed > 5)
        {
            Debug.Log(betList[0].GetRemainingPercentageLeft());
            timeElapsed = 0;
        }
    }

}
