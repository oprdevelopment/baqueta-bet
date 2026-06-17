using System;
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
    public FoulComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, Team comparedTeam) : base(multiplier, betManager, desiredCount, comparisonType)
    {
        this.comparedTeam = comparedTeam;
        foulComparisonType = FoulComparisonType.TeamOnly;
    }
    public FoulComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType, Player comparedPlayer) : base(multiplier, betManager, desiredCount, comparisonType)
    {
        this.comparedPlayer = comparedPlayer;
        foulComparisonType = FoulComparisonType.PlayerOnly;
    }
    public FoulComparisonBet(float multiplier, BetManager betManager, float desiredCount, ComparisonType comparisonType) : base(multiplier, betManager, desiredCount, comparisonType)
    {
        foulComparisonType = FoulComparisonType.All;
    }

    void GatherInfo(Player player)
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

    void EndGame(MatchInfo _) => VerifyBet();

    protected override void OnCreateBet()
    {
        GameSimulator.EventFoulCommited += GatherInfo;
        GameSimulator.EventGameEnded += EndGame;
    }

    protected override void OnEndBet()
    {
        GameSimulator.EventFoulCommited -= GatherInfo;
        GameSimulator.EventGameEnded -= EndGame;
    }
}
