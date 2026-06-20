using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class App : IVisualElementDisplay
    {
        public BetUiManager ctx;
        public VisualElement appContainer {get; private set;}
        Window currentWindow;
        Button closeButton;
        protected List<Window> windows = new();
        public App(BetUiManager betUiManager, VisualElement appContainer)
        {
            ctx = betUiManager;
            this.appContainer = appContainer;
            closeButton = appContainer.Q<Button>("CloseButton");
            closeButton.clicked += Hide;

           appContainer.Display(false);
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
        MatchInfo currentMatchInfo;
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
            ChangeMatchInfo(matchInfo);

            if(winnerTeam)
                titleLabel.text = $"{matchInfo.Home.name} x {matchInfo.Away.name}\r\nWin({winnerTeam.name})";
            else
                titleLabel.text = $"{matchInfo.Home.name} x {matchInfo.Away.name}\r\nDraw)";
            oddLabel.text = $"Odd: <color=green>{odd:F2}x";
            riskField.value = 0;
            rewardField.value = 0;

            riskField.RegisterValueChangedCallback(UpdateRiskField);
            rewardField.RegisterValueChangedCallback(UpdateRewardField);

            placeBetButton.clicked += () => 
            {
                if(riskField.value <= 0) return;
                currentBet.PlaceBet(riskField.value);
                Hide();
            };
        }
        void ChangeOnStateChange(MatchState state)
        {
            if(state != MatchState.Waiting && state != MatchState.Interval) Hide();
        }
        void ChangeMatchInfo(MatchInfo newInfo)
        {
            if(currentMatchInfo != null)
            {
                currentMatchInfo.MatchStateChange -= ChangeOnStateChange;
            }
            currentMatchInfo = newInfo;
            currentMatchInfo.MatchStateChange += ChangeOnStateChange;
        }
        void UpdateRiskField(ChangeEvent<float> evt)
        {
            if(evt.newValue < 0) riskField.SetValueWithoutNotify(0);
            rewardField.SetValueWithoutNotify((float)Math.Round(evt.newValue * currentBet.Multiplier, 2));
        }
        void UpdateRewardField(ChangeEvent<float> evt)
        {
            if(evt.newValue < 0) rewardField.SetValueWithoutNotify(0);
            riskField.SetValueWithoutNotify((float)Math.Round(evt.newValue / currentBet.Multiplier, 2));
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

            matchesButton = windowContainer.Q<Button>("MatchesButton");
            myBetsButton = windowContainer.Q<Button>("MyBetsButton");

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
            ctx.monitor.DeInteract();
            base.Hide();
        }
        public override void Show()
        {
            base.Show();
        }
    }
}