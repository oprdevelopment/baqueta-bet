using System;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;
public enum BetState
{
    Pending,
    InProgress,
    Won,
    Lost
}

public abstract class Bet
{

    public float Multiplier {get; private set;}
    public float Amount { get; private set; }
    protected MatchInfo matchInfo;
    BetState state;
    CurrencyManager currencyManager;
    BetManager betManager;
    public Bet(float multiplier, BetManager betManager, MatchInfo matchInfo)
    {
        this.Multiplier = multiplier;
        this.state = BetState.Pending;
        this.betManager = betManager;
        this.matchInfo = matchInfo;

    }
    public void ChangeMultiplier(float multiplier)
    {
        if(this.state != BetState.Pending) return;
        this.Multiplier = multiplier;
    }
    public void PlaceBet(float amount, CurrencyManager currencyManager)
    {
        if(this.state != BetState.Pending || !currencyManager.RemoveAmount(amount)) return;
        this.currencyManager = currencyManager;
        this.Amount = amount;

        this.state = BetState.InProgress;
        OnCreateBet();
    }
    public void EndBet()
    {
        VerifyBet();
    }
    protected void WinBet()
    {
        if(this.state != BetState.InProgress) return;
        currencyManager.AddAmount(Amount * Multiplier);
        this.state = BetState.Won;
    }
    protected void LoseBet()
    {
        if(this.state != BetState.InProgress) return;
        this.state = BetState.Lost;
    }
    protected abstract bool VerifyBet();
    protected abstract void OnCreateBet();
    protected abstract void OnEndBet();
}
