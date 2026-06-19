using System;
using UnityEngine;

public enum ComparisonType
{
    Equal,
    GreaterThan,
    LessThan
}
public enum IndividualComparisonType
{
    TeamOnly,
    PlayerOnly,
    All
}
public class ComparisonBet : Bet
{
    int currentCount = 0;
    float desiredCount;
    Team comparedTeam;
    Player comparedPlayer;
    ComparisonType comparisonType;
    Action<Player> playerDependentEvent;
    IndividualComparisonType invidualComparisonType;
    public ComparisonBet(BetManager betManager, MatchInfo matchInfo, CurrencyManager currencyManager, float multiplier, ComparisonType comparisonType, Action<Player> playerDependentEvent, float desiredCount) : base(betManager, matchInfo, currencyManager, multiplier)
    {
        this.comparisonType = comparisonType;
        invidualComparisonType = IndividualComparisonType.All;
    }
    public void SetIndividualComparisonType(Player player)
    {
        comparedPlayer = player;
    }
    public void SetIndividualComparisonType(Team team)
    {
        comparedTeam = team;
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
    protected override void OnCreateBet()
    {
        playerDependentEvent += OnPlayerDependentEvent;
    }

    protected override void OnEndBet()
    {
        playerDependentEvent -= OnPlayerDependentEvent;
    }
    void OnPlayerDependentEvent(Player player)
    {
        Debug.Log("Check Foul");
        switch (invidualComparisonType)
        {
            case IndividualComparisonType.All:
                IncreaseCounter(1);
                break;
            case IndividualComparisonType.PlayerOnly:
                if(player == comparedPlayer) IncreaseCounter(1);
                break;
            case IndividualComparisonType.TeamOnly:
                if(player.team == comparedTeam) IncreaseCounter(1);
                break;
        }
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
