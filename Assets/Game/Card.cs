public enum CardType
{
    None,
    Yellow,
    Red
}

public static class Card
{
    public static CardType RandomCard(float chance, CardType highestPossibleCard, CardType lowestPossibleCard)
    {
        float randomNumber = UnityEngine.Random.Range(0f, 1f);
        if(randomNumber < chance) return highestPossibleCard;
        return lowestPossibleCard;
    }
}