using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static event Action<CurrencyManager> BalanceChanged;
    public float Balance {get; private set;}
    public float initialBalance;
    void Start()
    {
        AddAmount(initialBalance);
    }
    public void AddAmount(float amount)
    {
        if(amount <= 0) return;
        Balance += (float) Math.Ceiling(amount * 100)/100;
        BalanceChanged?.Invoke(this);
    }

    public bool RemoveAmount(float amount)
    {
        if(Balance < amount || amount < 0) return false;
        Balance -= (float) Math.Ceiling(amount * 100)/100;
        BalanceChanged?.Invoke(this);
        return true;
    }
}
