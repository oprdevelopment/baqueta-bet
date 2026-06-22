using System;
public enum FoulIntensity
{
    Light = 41,
    Medium = 39,
    Tatical = 10,
    Heavy = 7,
    Brutal = 3,
}

[Serializable]
public class Foul
{
    public static FoulIntensity[] foulWeightList;
    public CardType cardGiven;
    public Player player;
    public Foul(CardType cardGiven, Player player)
    {
        this.cardGiven = cardGiven;
        this.player = player;
    }

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
