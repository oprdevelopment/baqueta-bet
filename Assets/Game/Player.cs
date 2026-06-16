[System.Serializable]
public class Player
{
    public Player (string name, Team team) {
        this.name = name;
        this.team = team;
        card = Card.None;
    }
    public string name;
    public Team team;
    public Card card;
}