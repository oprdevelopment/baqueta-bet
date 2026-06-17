using System;
using System.Linq;
using UnityEngine;
public class GameSimulator : MonoBehaviour
{
    public static event Action<Player> EventFoulCommited;
    public static event Action<Card, Player> EventCardGiven;
    public static event Action<Player> EventGoal;
    public static event Action<MatchInfo> EventGameEnded;
    MatchInfo matchInfo;
    float timeElapsed = 25;
    float foulBlock = 5;
    public void CommitFoul(FoulIntensity intensity, Player player)
    {
        var playerFouls = matchInfo.fouls.Where((p) => p.player == player).ToList().Count;
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
        matchInfo.fouls.Add(new(card, player));


        EventFoulCommited?.Invoke(player);
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
        if(matchInfo.ActivePlayers.Count <= 0) return;
        var randomPlayer = matchInfo.ActivePlayers[UnityEngine.Random.Range(0, matchInfo.ActivePlayers.Count)];

        CommitFoul(FoulRarity.RandomIntensity(), randomPlayer);
    }
    void GiveCard(Card card, Player player)
    {
        if(card == Card.None) return;

        EventCardGiven?.Invoke(card, player);

        if(player.card != Card.Yellow || card == Card.Yellow)
            player.card = card;
        else {
            player.card = Card.Red;
            EventCardGiven.Invoke(Card.Red, player);
        }

        EventCardGiven?.Invoke(card, player);

        if(player.card == Card.Red) matchInfo.InactivatePlayer(player);
    }

    void Start()
    {
        matchInfo = null;
        CreateNewMatch();
    }

    void CreateNewMatch()
    {
        matchInfo = new(Team.GenerateRandomTeam(), Team.GenerateRandomTeam());
    }
    void Update()
    {
        if(matchInfo == null) return;

        if(foulBlock < 0)
        {
            CommitRandomFoul();
            foulBlock = 5;
        }

        if(timeElapsed > 30) EventGameEnded?.Invoke(matchInfo);
        timeElapsed += Time.deltaTime;
        foulBlock -= Time.deltaTime;
    }
}
