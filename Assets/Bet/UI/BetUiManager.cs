using Assets.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
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
        public VisualTreeAsset matchCardTemplate, betCardTemplate;
        public BetApp betApp; 
        public PaymentApp paymentApp;
        public VisualElement root;
        UIDocument document;
        Label systemClock, moneyDisplay;
        public PlayerMovement playerMovement {get; private set;}
        public OpenBetApp monitor {get; private set;}
        Image mouse;
        bool mouseOnScreen;
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
            mouse = root.Q<Image>("Mouse");

            paymentApp = new PaymentApp(this, root.Q<VisualElement>("PaymentApp"));
            betApp = new BetApp(this, root.Q<VisualElement>("BetApp"));

            ClockManager.TickInfo += UpdateSystemClock;
            CurrencyManager.BalanceChanged += UpdateMoneyDisplay;

            root.RegisterCallback<PointerEnterEvent>(evt =>
            {
                mouseOnScreen = true;
            });
            root.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                mouseOnScreen = false;
            });
        }
        void Update()
        {
            if(!mouseOnScreen) return;

            Vector2 screenPos = Mouse.current.position.ReadValue();
            screenPos.y = Screen.height - screenPos.y;
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(mouse.panel, screenPos);
            mouse.style.left = panelPos.x;
            mouse.style.top = panelPos.y;
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
