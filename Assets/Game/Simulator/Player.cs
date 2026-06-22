using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public Player (string name, Team team) {
        this.name = name;
        this.team = team;
        card = CardType.None;
    }
    public string name;
    public Team team;
    public CardType card;
    public int goals;
    public static Player GetRandomPlayer(List<Player> players)
    {
        return players[UnityEngine.Random.Range(0, players.Count)];
    }
}