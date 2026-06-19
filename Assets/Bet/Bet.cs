using UnityEngine;
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

        public float Multiplier {get; private set;}
        public float Amount { get; private set; }
        protected MatchInfo matchInfo;
        BetState state;
        BetManager ctx;
        public Bet(BetManager betManager, MatchInfo matchInfo, float multiplier)
        {
            this.Multiplier = multiplier;
            this.state = BetState.Pending;
            this.ctx = betManager;
            this.matchInfo = matchInfo;

        }
        public void ChangeMultiplier(float multiplier)
        {
            if(this.state != BetState.Pending) return;
            this.Multiplier = multiplier;
        }
        public virtual void PlaceBet(float amount)
        {
            if(this.state != BetState.Pending || !ctx.currencyManager.RemoveAmount(amount)) return;
            this.Amount = amount;

            this.state = BetState.InProgress;
            OnCreateBet();

            matchInfo.MatchStateChange += EndBet;
            Debug.Log("Bet Placed");
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
            Debug.Log("Won");
        }
        protected void LoseBet()
        {
            this.state = BetState.Lost;
            Debug.Log("Lost");
        }
        protected abstract bool VerifyBet();
        protected abstract void OnCreateBet();
        protected abstract void OnEndBet();
    }
}