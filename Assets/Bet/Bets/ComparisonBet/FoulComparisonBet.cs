using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
public enum FoulComparisonType
{
    TeamOnly,
    PlayerOnly,
    All
}
public class FoulComparisonBet : ComparisonBet
{
    Team comparedTeam;
    Player comparedPlayer;
    FoulComparisonType foulComparisonType;
    public FoulComparisonBet(Team comparedTeam, float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo, CurrencyManager currencyManager) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo, currencyManager)
    {
        this.comparedTeam = comparedTeam;
        foulComparisonType = FoulComparisonType.TeamOnly;
    }
    public FoulComparisonBet(Player comparedPlayer, float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo, CurrencyManager currencyManager) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo, currencyManager)
    {
        this.comparedPlayer = comparedPlayer;
        foulComparisonType = FoulComparisonType.PlayerOnly;
    }
    public FoulComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo, CurrencyManager currencyManager) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo, currencyManager)
    {
        foulComparisonType = FoulComparisonType.All;
    }

    void OnFoulCommited(Player player)
    {
        Debug.Log("Check Foul");
        switch (foulComparisonType)
        {
            case FoulComparisonType.All:
                IncreaseCounter(1);
                break;
            case FoulComparisonType.PlayerOnly:
                if(player == comparedPlayer) IncreaseCounter(1);
                break;
            case FoulComparisonType.TeamOnly:
                if(player.team == comparedTeam) IncreaseCounter(1);
                break;
        }
    }

    protected override void OnCreateBet()
    {
        matchInfo.FoulCommited += OnFoulCommited;
    }

    protected override void OnEndBet()
    {
        matchInfo.FoulCommited -= OnFoulCommited;
    }
}
