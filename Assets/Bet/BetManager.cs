using System;
using System.Collections.Generic;
using UnityEngine;

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
            float timeRemaining = info.MatchState switch
            {
                MatchState.Waiting => 1,
                MatchState.Interval => 0.5f,
                _ => 0.5f
            };

            float homeScore = info.HomeScore;
            float awayScore = info.AwayScore;
            float scoreDifference = Mathf.Abs(homeScore - awayScore);

            float homeStrenght = info.Home.Strenght * timeRemaining;
            float awayStrenght = info.Away.Strenght * timeRemaining;

            float homeChance = 0.33f + (homeStrenght - awayStrenght) * 0.4f + (scoreDifference * 0.40f * (1.1f - timeRemaining));
            float awayChance = 0.33f + (awayStrenght - homeStrenght) * 0.4f - (scoreDifference * 0.40f * (1.1f - timeRemaining));
            float drawChance = scoreDifference == 0 
            ? .33f + (1 - timeRemaining) * .5f
            : .33f + Math.Abs(scoreDifference) * .3f * (1.1f - timeRemaining);

            float totalChance = homeChance + awayChance + drawChance;
            homeChance = Math.Clamp(homeChance / totalChance, 0.05f, 0.95f);
            awayChance = Math.Clamp(awayChance / totalChance, 0.05f, 0.95f);
            drawChance = Math.Clamp(drawChance / totalChance, 0.05f, 0.95f);

            return ((float)Math.Round(1/homeChance, 2), (float)Math.Round(1/awayChance, 2), (float)Math.Round(1/drawChance, 2));
        }
    }
}