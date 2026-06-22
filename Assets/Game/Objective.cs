using UnityEngine;

public class Objective : MonoBehaviour
{
    public float targetMoney;
    float suspiciounes;
    public static int MaxDays = 1;
    bool ganho;
    void Start()
    {
        ClockManager.DayPassed += day =>
        {
            if(targetMoney == 0 || suspiciounes > 0)
                Debug.Log("Perdeu");

            if(day >= MaxDays && !ganho)
                Debug.Log("Perd");
        };

        CurrencyManager.BalanceChanged += ctx =>
        {
            if(ctx.Balance > targetMoney)
            {
                ganho = true;  
                Debug.Log("Ganho");
            }
        };
    }
}
