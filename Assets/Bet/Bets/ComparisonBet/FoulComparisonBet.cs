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
    public FoulComparisonBet(Team comparedTeam, float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo)
    {
        this.comparedTeam = comparedTeam;
        foulComparisonType = FoulComparisonType.TeamOnly;
    }
    public FoulComparisonBet(Player comparedPlayer, float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo)
    {
        this.comparedPlayer = comparedPlayer;
        foulComparisonType = FoulComparisonType.PlayerOnly;
    }
    public FoulComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, MatchInfo matchInfo) : base(multiplier, betManager, desiredCount, comparisonType, matchInfo)
    {
        foulComparisonType = FoulComparisonType.All;
    }

    void OnFoulCommited(Player player)
    {
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

    void OnEndGame(MatchState _) => VerifyBet();

    protected override void OnCreateBet()
    {
        matchInfo.FoulCommited += OnFoulCommited;
        matchInfo.MatchStateChange += OnEndGame;
    }

    protected override void OnEndBet()
    {
        matchInfo.FoulCommited -= OnFoulCommited;
        matchInfo.MatchStateChange -= OnEndGame;
    }
}
