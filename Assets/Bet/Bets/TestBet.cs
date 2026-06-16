using UnityEngine;

public class TestBet : Bet
{
    public TestBet(float multiplier, BetManager betManager) : base(multiplier, betManager)
    {
    }
    protected override void VerifyBet()
    {
        Debug.Log($"Ganhou {this.Amount * this.Multiplier} com ${this.Amount} e {this.Multiplier}x");
        WinBet();
        OnEndBet();
    }

    protected override void OnCreateBet()
    {
        GameSimulator.TestAction += VerifyBet;
    }

    protected override void OnEndBet()
    {
        GameSimulator.TestAction -= VerifyBet;
    }
}