using System;
using UnityEngine;

namespace Assets.Bet.Bets
{

    public enum ComparisonType
    {
        Over,
        Under
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
        ComparisonType comparisonType;
        CardType cardType;
        public ComparisonBet(BetManager betManager, CardType cardType, MatchInfo matchInfo, float multiplier, ComparisonType comparisonType, float desiredCount) : base(betManager, matchInfo, multiplier)
        {
            this.comparisonType = comparisonType;
            this.cardType = cardType;
            this.desiredCount = desiredCount;
        }
        protected override void OnCreateBet()
        {
            matchInfo.CardGiven += OnCardGiven;
        }

        protected override void OnEndBet()
        {
            matchInfo.CardGiven -= OnCardGiven;
        }
        void OnCardGiven(CardType card, Player player)
        {
            if(card != cardType) return;
            Debug.Log("Check Foul");
            IncreaseCounter(1);
        }

        protected override bool VerifyBet()
        {
            var win = comparisonType switch
            {
                ComparisonType.Over => currentCount > desiredCount,
                ComparisonType.Under => currentCount < desiredCount,
                _ => false
            };

            if(win && comparisonType == ComparisonType.Over)
                WinBet();

            if(!win && comparisonType == ComparisonType.Under)
                LoseBet();

            return win;
        }
        protected void IncreaseCounter(int amount) {
            currentCount += amount;
            VerifyBet();
        }

        public override string GetBetType()
        {
            var comparison = comparisonType switch
            {
                ComparisonType.Over => ">",
                ComparisonType.Under => "<",
            };
            return $"{cardType} Card {comparison} {desiredCount}";
        }
    }
}