using System;
public enum FoulIntensity
{
    Light = 20,
    Medium = 19,
    Tatical = 15,
    Heavy = 5,
    Brutal = 2,
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
