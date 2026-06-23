using UnityEngine;

public class Objective : MonoBehaviour
{
    public float targetMoney;
    float suspiciounes;
    public static int MaxDays = 5;
    bool ganho;
    void Start()
    {
        ClockManager.DayPassed += day =>
        {
            if(targetMoney == 0 || suspiciounes > 0)
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);

            if(day >= MaxDays && !ganho)
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        };

        CurrencyManager.BalanceChanged += ctx =>
        {
            if(ctx.Balance > targetMoney)
            {
                ganho = true;  
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
        };
    }
}
