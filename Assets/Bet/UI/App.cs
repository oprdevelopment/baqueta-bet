using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class App : IVisualElementDisplay
    {
        public BetUiManager ctx;
        VisualElement appContainer;
        Window currentWindow;
        Button closeButton;
        protected List<Window> windows = new();
        public App(BetUiManager betUiManager, VisualElement appContainer)
        {
            ctx = betUiManager;
            this.appContainer = appContainer;
            closeButton = appContainer.Q<Button>("CloseButton");
            closeButton.clicked += Hide;

            Hide();
        }
        public void ChangeWindow(Window newWindow)
        {
            currentWindow?.Hide();
            newWindow.Show();
            currentWindow = newWindow;
        }

        public virtual void Show()
        {
            appContainer.Display(true);
        }

        public virtual void Hide()
        {
            appContainer.Display(false);
        }
    }
    public class PaymentApp : App
    {
        Bet currentBet = null;
        Label titleLabel, oddLabel;
        FloatField riskField, rewardField;
        Button placeBetButton;
        public PaymentApp(BetUiManager betUiManager, VisualElement appContainer) : base(betUiManager, appContainer)
        {
            titleLabel = appContainer.Q<Label>("Title");
            oddLabel = appContainer.Q<Label>("Odd");
            riskField = appContainer.Q<FloatField>("RiskField");
            rewardField = appContainer.Q<FloatField>("RewardField");
            placeBetButton = appContainer.Q<Button>("PlaceBetButton");
        }
        public void SetWinBet(MatchInfo matchInfo, float odd, Team winnerTeam)
        {
            currentBet = new WinBet(ctx.betManager, matchInfo, odd, winnerTeam);

            titleLabel.text = $"{matchInfo.Home.name} x {matchInfo.Away.name}\r\nWin({winnerTeam.name})";
            oddLabel.text = $"Odd: <color=green>{odd}x";
            riskField.value = 0;
            rewardField.value = 0;

            riskField.RegisterValueChangedCallback(UpdateRiskField);
            rewardField.RegisterValueChangedCallback(UpdateRewardField);

            placeBetButton.clicked += () => 
            {
                currentBet.PlaceBet(riskField.value);
                Hide();
            };
        }
        void UpdateRiskField(ChangeEvent<float> evt)
        {
            rewardField.SetValueWithoutNotify(evt.newValue * currentBet.Multiplier);
        }
        void UpdateRewardField(ChangeEvent<float> evt)
        {
            riskField.SetValueWithoutNotify(evt.newValue / currentBet.Multiplier);
        }
    }

    public class BetApp : App
    {
        public VisualElement windowContainer;
        public BetMakerWindow betMakerWindow;
        public Window matchSelectionWindow;
        public VisualTreeAsset matchCardTemplate;
        public Button matchesButton, myBetsButton;
        public BetApp(BetUiManager betUiManager, VisualElement appContainer) : base(betUiManager, appContainer)
        {
            windowContainer = appContainer.Q<VisualElement>("Window");
            matchSelectionWindow = new MatchSelectionWindow(this, windowContainer.Q<VisualElement>("MatchSelectionContainer"));
            betMakerWindow = new BetMakerWindow(this, windowContainer.Q<VisualElement>("BetMakerContainer"));

            matchesButton = windowContainer.Q<VisualElement>("TitleBar").Q<VisualElement>("TabsContainer").Q<Button>("MatchesButton");
            myBetsButton = windowContainer.Q<VisualElement>("TitleBar").Q<VisualElement>("TabsContainer").Q<Button>("MyBetsButton");

            matchesButton.clicked += () =>
            {
                ChangeWindow(matchSelectionWindow);  
            };
            
            windows.Add(matchSelectionWindow);
            windows.Add(betMakerWindow);

            ChangeWindow(matchSelectionWindow);
        }

        public override void Hide()
        {
            base.Hide();
        }
        public override void Show()
        {
            base.Show();
        }
    }
}