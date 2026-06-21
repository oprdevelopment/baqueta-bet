using UnityEngine;
using Assets;
using System;
namespace Assets.Bet
{
    public enum BetState
    {
        Pending,
        InProgress,
        Won,
        Lost
    }

    public abstract class Bet
    {
        public static Action<Bet> BetPlaced;
        public Action<BetState> StateChanged;
        public float Multiplier {get; private set;}
        public float Amount { get; private set; }
        public MatchInfo matchInfo;
        BetState state;
        BetManager ctx;
        public TimeInfo betTime;
        public abstract string GetBetType();
        public Bet(BetManager betManager, MatchInfo matchInfo, float multiplier)
        {
            this.Multiplier = multiplier;
            this.state = BetState.Pending;
            this.ctx = betManager;
            this.matchInfo = matchInfo;
            ClockManager.TickInfo += ctx => betTime = ctx;
        }
        public void ChangeMultiplier(float multiplier)
        {
            if(this.state != BetState.Pending) return;
            this.Multiplier = multiplier;
        }
        public virtual void PlaceBet(float amount, string betType)
        {
            if(this.state != BetState.Pending || !ctx.currencyManager.RemoveAmount(amount)) return;
            this.Amount = amount;

            this.state = BetState.InProgress;
            OnCreateBet();

            matchInfo.MatchStateChange += EndBet;
            BetPlaced?.Invoke(this);            
        }
        public void EndBet(MatchState matchState)
        {
            if(matchState != MatchState.MatchEnded) return;
            
            if (VerifyBet()) WinBet();
            else LoseBet();

            OnEndBet();
            matchInfo.MatchStateChange -= EndBet;
        }
        protected void WinBet()
        {
            ctx.currencyManager.AddAmount(Amount * Multiplier);
            this.state = BetState.Won;
            StateChanged?.Invoke(state);
        }
        protected void LoseBet()
        {
            this.state = BetState.Lost;
            StateChanged?.Invoke(state);
        }
        protected abstract bool VerifyBet();
        protected abstract void OnCreateBet();
        protected abstract void OnEndBet();
    }
}