using UnityEngine;
using UnityEngine.UIElements;

public class DinheiroDinheiro : MonoBehaviour
{
    UIDocument document;
    VisualElement root;
    Label dinheiro, day;
    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        dinheiro = root.Q<Label>("Dinero");
        day = root.Q<Label>("Day");
        this.day.text = $"Day 1/{Objective.MaxDays}";  

        CurrencyManager.BalanceChanged += ctx =>
        {
            dinheiro.text = $"{ctx.Balance:F2}";
        };
        ClockManager.DayPassed += day =>
        {
            this.day.text = $"Day {day + 1}/{Objective.MaxDays}";  
        };
    }
}
