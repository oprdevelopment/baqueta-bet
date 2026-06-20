using Assets.Interaction;
using UnityEngine;
using UnityEngine.UIElements;

public static class IntExtension
{
    public static string Formated(this int number) => number < 10 ? $"0{number}" : number.ToString(); 
}

namespace Assets.Bet.UI
{
    public static class UIExtensions
    {
        public static void Display(this VisualElement element, bool show) => element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
    public interface IVisualElementDisplay
    {
        public void Show();
        public void Hide();
    }
    public class BetUiManager : MonoBehaviour
    {
        public BetManager betManager;
        public VisualTreeAsset matchCardTemplate;
        public BetApp betApp; 
        public PaymentApp paymentApp;
        public VisualElement root;
        UIDocument document;
        Label systemClock, moneyDisplay;
        public PlayerMovement playerMovement;
        public OpenBetApp monitor {get; private set;}
        void Awake()
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            monitor = FindAnyObjectByType<OpenBetApp>();
        }
        private void OnEnable()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            systemClock = root.Q<Label>("Clock");
            moneyDisplay = root.Q<Label>("MoneyDisplay");

            paymentApp = new PaymentApp(this, root.Q<VisualElement>("PaymentApp"));
            betApp = new BetApp(this, root.Q<VisualElement>("BetApp"));

            ClockManager.TickInfo += UpdateSystemClock;
            CurrencyManager.BalanceChanged += UpdateMoneyDisplay;
            betApp.Show();
        }

        void OnDisable()
        {
            ClockManager.TickInfo -= UpdateSystemClock;
        }

        void UpdateSystemClock(TimeInfo timeInfo)
        {
            systemClock.text = timeInfo.FormatedDay('/') + " " + timeInfo.FormatedTime(':');
        }
        void UpdateMoneyDisplay(CurrencyManager currencyManager)
        {
            moneyDisplay.text = $"${currencyManager.Balance:F2}";
        }
    }
}
