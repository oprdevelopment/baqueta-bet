using System;
using System.Collections.Generic;
using System.Linq;
public enum Card
{
    None,
    Yellow,
    Red
}
public enum FoulIntensity
{
    Light = 16,
    Medium = 8,
    Tatical = 4,
    Heavy = 2,
    Brutal = 1,
}
public class FoulRarity
{
    public static FoulIntensity[] foulWeightList;
    public static FoulIntensity RandomIntensity()
    {
        foulWeightList ??= (FoulIntensity[])Enum.GetValues(typeof(FoulIntensity));

        int totalWeight = 0;
        foreach(var i in foulWeightList)
            totalWeight += (int)i;

        int randomNumber = UnityEngine.Random.Range(0, totalWeight);

        int accWeight = 0;
        foreach(var i in foulWeightList)
        {
            accWeight += (int)i;
            if(randomNumber < accWeight)
                return i;
        }

        return FoulIntensity.Light;
    }
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

[Serializable]
public class MatchInfo
{
    public Team home, away;
    public List<Player> ActivePlayers {get; private set;}
    public int homeScore, awayScore;
    public List<Foul> fouls;
    public MatchInfo(Team home, Team away)
    {
        this.home = home;
        this.away = away;
        homeScore = 0;
        awayScore = 0;
        fouls = new();
        ActivePlayers = GetAllActivePlayers();
    }

    public void InactivatePlayer(Player player)
    {
        ActivePlayers.Remove(player);
    }

    List<Player> GetAllActivePlayers()
    {
        return home.players.Concat(away.players).ToList();
    }
}