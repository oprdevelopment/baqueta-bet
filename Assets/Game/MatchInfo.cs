using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MatchState
{
    Waiting,
    FirstHalf,
    Interval,
    SecondHalf,
    MatchEnded
}

[Serializable]
public class MatchInfo
{
    readonly MatchManager matchManager;
    readonly List<Player> activePlayers;
    public Action<Player> FoulCommited;
    public Action<CardType, Player> CardGiven;
    public Action<Player> GoalScoredInfo;
    public Action GoalScored;
    public Action<MatchState> MatchStateChange;
    public Team Home {private set; get;}
    public Team Away {private set; get;}
    public int HomeScore {private set; get;}
    public int AwayScore {private set; get;}
    public List<Foul> Fouls {private set; get;}
    public MatchState MatchState {private set; get;}
    public TimeInfo StartTime {private set; get;}
    public int GameTime {private set; get;}
    int extraTime = 0;
    public MatchInfo(Team home, Team away, MatchManager matchManager)
    {
        this.Home = home;
        this.Away = away;
        this.matchManager = matchManager;
        HomeScore = 0;
        AwayScore = 0;
        GameTime = 0;
        Fouls = new();
        MatchState = MatchState.Waiting;
        activePlayers = GetAllActivePlayers();
        
        int[] possibleMinutes = {0, 30, 45};
        StartTime = new TimeInfo
        {
            Hours = UnityEngine.Random.Range(10, 22),
            Minutes = possibleMinutes[UnityEngine.Random.Range(0, possibleMinutes.Length)],
            Day = UnityEngine.Random.Range(0, 3)
        };

        ClockManager.Tick += OnTick;
        ClockManager.TickInfo += OnTickInfo;
    }
    void OnTick()
    {
        if(MatchState == MatchState.Waiting || MatchState == MatchState.MatchEnded) return;

        if(GameTime >= GetStateDuration(MatchState) + extraTime)
        {
            ChangeState(GetNextState());
            GameTime = 0;
        }
        GameTime++;

        if(MatchState != MatchState.SecondHalf && MatchState != MatchState.FirstHalf)
            return;

        float timePassed = this.MatchState switch
        {
            MatchState.FirstHalf => GameTime,
            MatchState.SecondHalf => 45 + GameTime,
            _ => 0
        };

        float combinedStrenght = Home.Strenght + Away.Strenght;
        float goalScoredChance = combinedStrenght * (1 + timePassed / 90f / 4) / 2500 / 2;

        float randomTick = UnityEngine.Random.Range(0f, 1f);
        if(randomTick <= goalScoredChance)
            RandomGoal(combinedStrenght);

        
        Debug.Log(goalScoredChance);

    }
    void RandomGoal(float combinedStrenght)
    {
        float homeChance = Home.Strenght / combinedStrenght;

        if(UnityEngine.Random.Range(0f, 1f) < homeChance)
            ScoreGoal(Player.GetRandomPlayer(activePlayers.Where(p => p.team == Home).ToList()));
        else
            ScoreGoal(Player.GetRandomPlayer(activePlayers.Where(p => p.team == Away).ToList()));


    }
    void RandomFoul()
    {
        CommitFoul(Foul.RandomIntensity(), Player.GetRandomPlayer(activePlayers));
    }
    void OnTickInfo(TimeInfo tickInfo)
    {
        if(MatchState != MatchState.Waiting) return;

        if(tickInfo == StartTime)
        {
            ChangeState(MatchState.FirstHalf);
        }
    }
    void ChangeState(MatchState state)
    {
        MatchState = state;
        MatchStateChange?.Invoke(state);
        extraTime = UnityEngine.Random.Range(1, 8);
    }
    public static int GetStateDuration(MatchState state)
    {
        return state switch
        {
            MatchState.Interval => 15,
            _ => 45,
        };
    }
    public MatchState GetNextState()
    {
        if(MatchState == MatchState.MatchEnded) return MatchState.MatchEnded;

        var currentInt = (int)MatchState;
        return (MatchState)(currentInt + 1);
    }
    public void InactivatePlayer(Player player)
    {
        activePlayers.Remove(player);
    }
    List<Player> GetAllActivePlayers()
    {
        return Home.players.Concat(Away.players).ToList();
    }
    void GiveCard(CardType card, Player player)
    {
        if(card == CardType.None) return;

        CardGiven?.Invoke(card, player);

        if(player.card != CardType.Yellow || card != CardType.Yellow)
            player.card = card;
        else {
            player.card = CardType.Red;
            CardGiven?.Invoke(CardType.Red, player);
        }

        if(player.card == CardType.Red) InactivatePlayer(player);
    }
    public void CommitFoul(FoulIntensity intensity, Player player)
    {
        var playerFouls = Fouls.Where((p) => p.player == player).ToList().Count;
        var chance = 0.20f * playerFouls;
        var card = CardType.None;

        switch (intensity)
        {
            case FoulIntensity.Light:
                break;
            case FoulIntensity.Tatical:
                chance += 1;
                card = CardType.Yellow;
                break;
            case FoulIntensity.Medium:
                chance += 0.2f;
                card = Card.RandomCard(chance, CardType.Yellow, CardType.None);
                break;
            case FoulIntensity.Heavy:
                chance += 0.35f;
                card = Card.RandomCard(chance, CardType.Red, CardType.Yellow);
                break;
            case FoulIntensity.Brutal:
                chance += 1;
                card = CardType.Red;
                break;
        }

        GiveCard(card, player);
        Fouls.Add(new(card, player));


        FoulCommited?.Invoke(player);
    }
    public void ScoreGoal(Player player)
    {
        if(player.team == Home) HomeScore++;
        if(player.team == Away) AwayScore++;

        GoalScored?.Invoke();
    }
}