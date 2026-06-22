using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Bet.Bets;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

namespace Assets.Bet
{
    public class BetManager : MonoBehaviour
    {
        public static BetManager Instance;
        public CurrencyManager currencyManager;
        [SerializeField] MatchManager gameSimulator;
        List<Bet> betList;
        [SerializeField] float baseOdd = 1.01f;
        [SerializeField] float winOddstrenghtInfluence = 1.5f; 
        [SerializeField] float winOddGoalInfluence = 1.5f;
        [SerializeField] float winOddDrawChance = 0.25f;
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        void Start()
        {
            betList = new();
        }

        private static float ToOdd(float probability)
        {
            return (float ) Math.Round(probability <= 0.001f
                ? 999f
                : 1f / probability, 2);
        }

        public static (float oddGreater, float oddLess) CalculateOddCard(MatchInfo info, CardType cardType, int targetCards)
        {
            int totalCards = info.Fouls.Count(f => f.cardGiven == cardType);

            float progress = Math.Clamp(info.GetRealGameTime() / 90f, 0f, 1f);
            float remaining = 1f - progress;

            float averagePerGame = cardType switch
            {
                CardType.Yellow => 6f,
                CardType.Red => 0.4f,
                _ => 5f
            };

            float expectedRemaining = averagePerGame * remaining;
            float expectedFinal = totalCards + expectedRemaining;

            float distance = expectedFinal - targetCards;

            float over = 0.5f + distance * 0.1f;

            over = Math.Clamp(over, 0.01f, 0.99f);

            float under = 1f - over;
            return (
                ToOdd(over),
                ToOdd(under)
            );
        }

        public static (float oddHome, float oddAway, float oddDraw) CalculateWinOdd(MatchInfo info)
        {
            float minutesPlayed = info.MatchState switch
            {
                MatchState.FirstHalf => info.GameTime,
                MatchState.SecondHalf => info.GameTime + 45,
                _ => 0
            };

            float scoreDiference = info.HomeScore - info.AwayScore;
            float timeFactor = 10 + minutesPlayed / 90 * 50;

            float pointsHome = info.Home.Strenght;
            float pointsAway = info.Away.Strenght;

            if(scoreDiference > 0)
                pointsHome += scoreDiference * timeFactor;
            else if(scoreDiference < 0)
                pointsAway -= scoreDiference * timeFactor;

            float drawPoints = 25f;
            drawPoints *= 1 - minutesPlayed/90 * 0.5f;
            drawPoints = Math.Max(drawPoints, 1f);


            float totalPoints = pointsHome + pointsAway + drawPoints;

            float homeChance = pointsHome / totalPoints;
            float awayChance = pointsAway / totalPoints;
            float drawChance = drawPoints / totalPoints;

            float margin = 0.95f;

            float oddHome = (float) Math.Round(margin / homeChance, 2);
            float oddAway = (float) Math.Round(margin / awayChance, 2);
            float oddDraw = (float) Math.Round(margin / drawChance, 2);

            return (oddHome, oddAway, oddDraw);
        }
    }
}