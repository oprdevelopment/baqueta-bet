using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public enum Card
{
    None,
    Yellow,
    Red
}
public enum FoulIntensity
{
    Light,
    Tatical,
    Medium,
    Heavy,
    Brutal,
}

[Serializable]
public class GameInfo
{
    public Team home, away;
    public int homeScore, awayScore;
    public List<Foul> fouls;
}

[Serializable]
public class Foul
{
    public Card cardGiven;
    public Player player;
    public Foul(Card cardGiven, Player player)
    {
        this.cardGiven = cardGiven;
        this.player = player;
    }
}

public class GameSimulator : MonoBehaviour
{
    public static event Action<Player> FoulCommited;
    public static event Action<Card, Player> CardGiven;
    public static event Action Goal;
    public static event Action<GameInfo> GameEnded;
    public static event Action TestAction;
    public GameInfo gameInfo;
    float timeElapsed = 25;
    float foulBlock = 5;
    public void CommitFoul(FoulIntensity intensity, Player player)
    {
        var playerFouls = gameInfo.fouls.Where((p) => p.player == player).ToList().Count;
        var chance = 0.20f * playerFouls;
        var card = Card.None;

        switch (intensity)
        {
            case FoulIntensity.Light:
                break;
            case FoulIntensity.Tatical:
                chance += 1;
                card = Card.Yellow;
                break;
            case FoulIntensity.Medium:
                chance += 0.2f;
                card = RandomCard(chance, Card.Yellow, Card.None);
                break;
            case FoulIntensity.Heavy:
                chance += 0.35f;
                card = RandomCard(chance, Card.Red, Card.Yellow);
                break;
            case FoulIntensity.Brutal:
                chance += 1;
                card = Card.Red;
                break;
        }

        GiveCard(card, player);

        if(card == Card.Yellow && player.card == Card.Yellow)
            GiveCard(Card.Red, player); 

        gameInfo.fouls.Add(new(card, player));
        FoulCommited?.Invoke(player);

        Debug.Log($"{player.name} -> {intensity} : {card}");
    }
    Card RandomCard(float chance, Card highestPossibleCard, Card lowestPossibleCard)
    {
        float randomNumber = UnityEngine.Random.Range(0f, 1f);
        if(randomNumber < chance) return highestPossibleCard;
        return lowestPossibleCard;
    }
    void CommitRandomFoul()
    {
        var activePlayers = GetActivePlayers();
        if(activePlayers.Count <= 0) return;
        var randomPlayer = activePlayers[UnityEngine.Random.Range(0, activePlayers.Count)];
        var randomIntensity = (FoulIntensity)UnityEngine.Random.Range(0, Enum.GetValues(typeof(FoulIntensity)).Length);

        CommitFoul(randomIntensity, randomPlayer);
    }
    void GiveCard(Card card, Player player)
    {
        if(card == Card.None) return;
        player.card = card;
        CardGiven?.Invoke(card, player);
    }
    List<Player> GetActivePlayers()
    {
        return gameInfo.home.players.Where((p) => p.card != Card.Red).ToList().Concat(
        gameInfo.away.players.Where(p => p.card != Card.Red).ToList()).ToList();
    }

    void Update()
    {
        if(foulBlock < 0)
        {
            CommitRandomFoul();
            foulBlock = 5;
        }

        if(timeElapsed > 30) GameEnded?.Invoke(gameInfo);
        timeElapsed += Time.deltaTime;
        foulBlock -= Time.deltaTime;
    }
}
