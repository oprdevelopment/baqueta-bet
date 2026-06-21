using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

        // public static (float oddGreater, float oddLess, float oddEqual) CalculateGoalsOdd(MatchInfo info, int targetGoals, IndividualComparisonType individual = IndividualComparisonType.All, Team individualTeam = null, Player individualPlayer= null)
        // {
        //     float timeRemaining = info.MatchState switch
        //     {
        //         MatchState.Waiting => 1,
        //         MatchState.Interval => 0.5f,
        //         _ => 0.5f
        //     };

        //     float mediaHome = (.8f + (2.8f * (info.Home.Strenght - 0.1f) / 0.9f)) * timeRemaining;
        //     float mediaAway = (.2f + (2.8f * (info.Away.Strenght - 0.1f) / 0.9f)) * timeRemaining;

        //     int currentGoals = individual switch
        //     {
        //         IndividualComparisonType.All => info.AwayScore + info.HomeScore,
        //         IndividualComparisonType.TeamOnly => individualTeam == info.Home ? info.HomeScore : info.AwayScore,
        //         IndividualComparisonType.PlayerOnly => individualPlayer.goals
        //     };

        //     float totalStrengt = individual switch
        //     {
        //         IndividualComparisonType.All => mediaAway + mediaHome,
        //         IndividualComparisonType.TeamOnly => individualTeam == info.Home ? mediaHome : mediaAway,
        //         IndividualComparisonType.PlayerOnly => individualPlayer.team == info.Home ? mediaHome : mediaAway
        //     };

        //     int remainingGoals = targetGoals - currentGoals;

        //     float difference = totalStrengt - remainingGoals;

        //     float chanceGreater = .5f + (difference * .25f);
        //     float chanceEqual = .25f - (Math.Abs(difference) * .15f);
        //     float chanceLess = 1 - chanceEqual - chanceGreater;

        //     chanceGreater = Math.Clamp(chanceGreater, 0.05f, 0.99f);
        //     chanceLess = Math.Clamp(chanceLess, 0.05f, 0.99f);
        //     chanceEqual = Math.Clamp(chanceEqual, 0.05f, 0.99f);

        //     return (1 / chanceGreater, 1 / chanceLess, 1 / chanceEqual);
        // }

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