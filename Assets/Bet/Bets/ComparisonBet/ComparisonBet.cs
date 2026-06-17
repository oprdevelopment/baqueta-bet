using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum ComparisonType
{
    Equal,
    GreaterThan,
    LessThan
}
public abstract class ComparisonBet : Bet
{
    int currentCount = 0;
    float desiredCount;
    ComparisonType comparisonType;
    protected ComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo, CurrencyManager currencyManager) : base(multiplier, betManager, matchInfo, currencyManager)
    {
        ChangeDesiredAmount(desiredCount);
        this.comparisonType = comparisonType;
    }

    public void ChangeDesiredAmount(float newAmount)
    {
        this.desiredCount = comparisonType switch
        {
            ComparisonType.GreaterThan => (int)Mathf.Ceil(newAmount),
            ComparisonType.LessThan => (int)Math.Floor(newAmount),
            _ => (int)newAmount
        };
    }

    protected override bool VerifyBet()
    {
        var win = comparisonType switch
        {
            ComparisonType.Equal => currentCount == desiredCount,
            ComparisonType.GreaterThan => currentCount >= desiredCount,
            ComparisonType.LessThan => currentCount <= desiredCount,
            _ => false
        };

        if(win && comparisonType == ComparisonType.GreaterThan)
            WinBet();

        if(!win && comparisonType == ComparisonType.LessThan)
            LoseBet();

        return win;
    }
    protected void IncreaseCounter(int amount) {
        currentCount += amount;
        VerifyBet();
    }
}
