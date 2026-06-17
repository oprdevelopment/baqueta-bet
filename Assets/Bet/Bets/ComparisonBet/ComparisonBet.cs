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
    protected ComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo) : base(multiplier, betManager, matchInfo)
    {
        this.desiredCount = desiredCount;
        this.comparisonType = comparisonType;
    }

    protected override bool VerifyBet()
    {
        return comparisonType switch
        {
            ComparisonType.Equal => currentCount == desiredCount,
            ComparisonType.GreaterThan => currentCount > desiredCount,
            ComparisonType.LessThan => currentCount < desiredCount,
            _ => false
        };
    }
    protected void IncreaseCounter(int amount) => currentCount += amount;
    public virtual float GetRemainingPercentageLeft() => currentCount / desiredCount;
}
