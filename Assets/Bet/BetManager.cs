using System;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    [SerializeField] CurrencyManager currencyManager;
    List<TestBet> betList;
    public float timeElapsed;
    void Start()
    {
        betList = new();
    }
}
