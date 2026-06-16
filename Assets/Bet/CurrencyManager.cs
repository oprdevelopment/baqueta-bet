using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public float Balance;
    public void AddAmount(float amount)
    {
        Balance += amount;
    }

    public bool RemoveAmount(float amount)
    {
        if(Balance < amount) return false;
        Balance -= amount;
        return true;
    }
}
