using UnityEngine;
using UnityEngine.UIElements;

public class DinheiroDinheiro : MonoBehaviour
{
    UIDocument document;
    VisualElement root;
    Label dinheiro;
    void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        dinheiro = root.Q<Label>("Dinero");

        CurrencyManager.BalanceChanged += ctx =>
        {
            dinheiro.text = $"{ctx.Balance:F2}";
        };
    }
}
