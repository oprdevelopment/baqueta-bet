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
    public FoulComparisonBet(BetManager betManager, MatchInfo matchInfo, CurrencyManager currencyManager, float multiplier, ComparisonType comparisonType, float desiredCount, Team comparedTeam) : base(betManager, matchInfo, currencyManager, multiplier, comparisonType, desiredCount)
    {
        this.comparedTeam = comparedTeam;
        foulComparisonType = FoulComparisonType.TeamOnly;
    }
    public FoulComparisonBet(BetManager betManager, MatchInfo matchInfo, CurrencyManager currencyManager, float multiplier, ComparisonType comparisonType, float desiredCount, Player comparedPlayer) : base(betManager, matchInfo, currencyManager, multiplier, comparisonType, desiredCount)
    {
        this.comparedPlayer = comparedPlayer;
        foulComparisonType = FoulComparisonType.PlayerOnly;
    }
    public FoulComparisonBet(BetManager betManager, MatchInfo matchInfo, CurrencyManager currencyManager, float multiplier, ComparisonType comparisonType, float desiredCount) : base(betManager, matchInfo, currencyManager, multiplier, comparisonType, desiredCount)
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
