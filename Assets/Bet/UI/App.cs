using System;
using System.Collections.Generic;
using Assets.Bet.Bets;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
            newWindow?.Show();
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

            riskField.RegisterValueChangedCallback(UpdateRiskField);
            rewardField.RegisterValueChangedCallback(UpdateRewardField);
            placeBetButton.clicked += () => 
            {
                if(riskField.value <= 0) return;
                if(!currentBet.PlaceBet(riskField.value, currentBet.GetBetType())) return;
                Hide();
            };
        }
        internal void SetCardBet(CardType card, float odd, MatchInfo matchInfo, ComparisonType comparisonType, float desiredAmount)
        {
            currentBet = new ComparisonBet(ctx.betManager, card, matchInfo, odd, comparisonType, desiredAmount);
            ChangeMatchInfo(matchInfo);

            titleLabel.text = $"{matchInfo.Home.name} x {matchInfo.Away.name}\r\n{currentBet.GetBetType()}";

            oddLabel.text = $"Odd: <color=green>{odd:F2}x";
            riskField.value = 0;
            rewardField.value = 0;
        }
        public void SetWinBet(MatchInfo matchInfo, float odd, Team winnerTeam)
        {
            currentBet = new WinBet(ctx.betManager, matchInfo, odd, winnerTeam);
            ChangeMatchInfo(matchInfo);

            titleLabel.text = $"{matchInfo.Home.name} x {matchInfo.Away.name}\r\n{currentBet.GetBetType()}";

            oddLabel.text = $"Odd: <color=green>{odd:F2}x";
            riskField.value = 0;
            rewardField.value = 0;
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
            if(currentBet == null) return;
            if(evt.newValue < 0) riskField.SetValueWithoutNotify(0);
            rewardField.SetValueWithoutNotify((float)Math.Round(evt.newValue * currentBet.Multiplier, 2));
        }
        void UpdateRewardField(ChangeEvent<float> evt)
        {
            if(currentBet == null) return;
            if(evt.newValue < 0) rewardField.SetValueWithoutNotify(0);
            riskField.SetValueWithoutNotify((float)Math.Round(evt.newValue / currentBet.Multiplier, 2));
        }

    }

    public class BetApp : App
    {
        public VisualElement windowContainer;
        public BetMakerWindow betMakerWindow;
        public MatchSelectionWindow matchSelectionWindow;
        public BetVisualizationWindow betVisualizationWindow;
        public VisualTreeAsset matchCardTemplate;
        public Button matchesButton, myBetsButton;
        public BetApp(BetUiManager betUiManager, VisualElement appContainer) : base(betUiManager, appContainer)
        {
            windowContainer = appContainer.Q<VisualElement>("Window");
            matchSelectionWindow = new MatchSelectionWindow(this, windowContainer.Q<VisualElement>("MatchSelectionContainer"));
            betMakerWindow = new BetMakerWindow(this, windowContainer.Q<VisualElement>("BetMakerContainer"));
            betVisualizationWindow = new BetVisualizationWindow(this, windowContainer.Q<VisualElement>("MyBetsContainer"));

            matchesButton = windowContainer.Q<Button>("MatchesButton");
            myBetsButton = windowContainer.Q<Button>("MyBetsButton");

            matchesButton.clicked += () =>
            {
                ChangeWindow(matchSelectionWindow);  
            };
            myBetsButton.clicked += () =>
            {
                ChangeWindow(betVisualizationWindow);
            };
            
            windows.Add(matchSelectionWindow);
            windows.Add(betMakerWindow);
            windows.Add(betVisualizationWindow);
        }
        public override void Hide()
        {
            ctx.monitor.DeInteract();
            base.Hide();
        }
        public override void Show()
        {
            ChangeWindow(matchSelectionWindow);
            base.Show();
        }
    }
}