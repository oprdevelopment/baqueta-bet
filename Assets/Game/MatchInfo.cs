using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public enum MatchState
{
    PreMatch,
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
    public event Action<Player> FoulCommited;
    public event Action<CardType, Player> CardGiven;
    public event Action<Player> Goal;
    public event Action<MatchState> MatchStateChange;
    public Team Home {private set; get;}
    public Team Away {private set; get;}
    public int HomeScore {private set; get;}
    public int AwayScore {private set; get;}
    public List<Foul> Fouls {private set; get;}
    public MatchState MatchState {private set; get;}
    float tickCount;
    public MatchInfo(Team home, Team away, MatchManager matchManager)
    {
        this.Home = home;
        this.Away = away;
        this.matchManager = matchManager;
        HomeScore = 0;
        AwayScore = 0;
        tickCount = 0;
        Fouls = new();
        MatchState = MatchState.PreMatch;
        activePlayers = GetAllActivePlayers();

        MatchManager.Tick += OnTick;
    }

    void OnTick()
    {
        if(tickCount >= GetStateDuration(MatchState))
        {
            ChangeState(GetNextState());
            tickCount = 0;
        }
        tickCount++;

        if(tickCount % 5 == 0)
        {
            CommitFoul(Foul.RandomIntensity(), Player.GetRandomPlayer(activePlayers));
        }
    }

    void ChangeState(MatchState state)
    {
        MatchState = state;
        MatchStateChange?.Invoke(state);
    }

    public static int GetStateDuration(MatchState state)
    {
        #if UNITY_EDITOR
        return state switch
        {
            MatchState.PreMatch => 10,
            MatchState.Interval => 10,
            _ => 60,
        };
        #endif
        #if !UNITY_EDITOR
        return state switch
        {
            MatchState.PreMatch => 90,
            MatchState.Interval => 90,
            _ => 360,
        };
        #endif
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

        CardGiven?.Invoke(card, player);

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
        Debug.Log($"Foul Commited by {player.name}: {player.team} -> {intensity} : {card}");
    }
}