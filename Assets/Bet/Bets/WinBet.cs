namespace Assets.Bet.Bets
{       
    public class WinBet : Bet
    {
        Team winnerTeam;
        public WinBet(BetManager betManager, MatchInfo matchInfo, float multiplier, Team winnerTeam) : base(betManager, matchInfo, multiplier)
        {
            this.winnerTeam = winnerTeam;
        }

        public override string GetBetType()
        {
            return winnerTeam != null ? $"Win ({winnerTeam.name})" : "Draw";
        }

        protected override void OnCreateBet()
        {
        }

        protected override void OnEndBet()
        {
        }

        protected override bool VerifyBet()
        {
            if(matchInfo.MatchState != MatchState.MatchEnded) return false;

            Team winner = null;

            if(matchInfo.HomeScore < matchInfo.AwayScore)
                winner = matchInfo.Away;
            if(matchInfo.HomeScore > matchInfo.AwayScore)
                winner = matchInfo.Home;

            return winnerTeam == winner;
        }
    }
}