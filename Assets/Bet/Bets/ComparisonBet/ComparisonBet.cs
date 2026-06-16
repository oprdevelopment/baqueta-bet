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
    protected ComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType) : base(multiplier, betManager)
    {
        this.desiredCount = desiredCount;
        this.comparisonType = comparisonType;
    }

    protected override void VerifyBet()
    {
        var win = false;
        switch (comparisonType)
        {
            case ComparisonType.Equal:
                win = currentCount == desiredCount;
                break;
            case ComparisonType.GreaterThan:
                win = currentCount > desiredCount;
                break;
            case ComparisonType.LessThan:
                win = currentCount < desiredCount;
                break;
        }

        if(win) WinBet();
        else LoseBet();
    }
    protected void IncreaseCounter(int amount) => currentCount += amount;
    public virtual float GetRemainingPercentageLeft() => currentCount / desiredCount;
}
