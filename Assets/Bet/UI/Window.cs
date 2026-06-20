using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Assets.Bet.UI
{

    public abstract class Window : IVisualElementDisplay
    {
        protected VisualElement window;
        public Window(App app, VisualElement windowContainer)
        {
            this.window = windowContainer;
            Hide();
        }
        public virtual void Hide()
        {
            window.Display(false);
        }

        public virtual void Show()
        {
            window.Display(true);
        }
    }
    public class MatchSelectionWindow : Window
    {
        public List<MatchCard> matchCards = new();
        BetApp betApp;
        VisualElement matchScrollView;
        public MatchSelectionWindow(BetApp app, VisualElement windowContainer) : base(app, windowContainer)
        {
            this.betApp = app;
            matchScrollView = windowContainer.Q<ScrollView>("MatchScrollView");

            MatchManager.CreatedMatch += OnCreatedMatch;
        }
        public void UpdateCards()
        {
            matchScrollView.Clear();
            matchCards.Sort((a, b) => a.matchInfo.StartTime.CompareTo(b.matchInfo.StartTime));
            matchCards.ForEach(c => {
                c.Hide();
                matchScrollView.Add(c.Card);
                c.Show();
            });        
        }
        void OnCreatedMatch(MatchInfo matchInfo)
        {
            MatchCard newMatchCard = new(betApp, matchInfo);
            matchCards.Add(newMatchCard);
            matchScrollView.Add(newMatchCard.Card);

            UpdateCards();
        }
    }
    public class BetMakerWindow : Window
    {
        Button homeButton, awayButton, drawButton;
        Label timeDisplay;
        BetApp betApp;
        public VisualElement BetMakerContainer;
        public List<MatchCard> matchCards = new();
        float oddHome, oddAway, oddDraw;
        MatchInfo currentMatchInfo;

        public BetMakerWindow(BetApp betApp, VisualElement window) : base(betApp, window)
        {
            this.betApp = betApp;

            homeButton = window.Q<Button>("HomeWinButton");
            awayButton = window.Q<Button>("AwayWinButton");
            drawButton = window.Q<Button>("DrawButton");
            timeDisplay = window.Q<Label>("Time");

            homeButton.clicked += () => {
                betApp.ctx.paymentApp.SetWinBet(currentMatchInfo, oddHome, currentMatchInfo.Home);
                betApp.ctx.paymentApp.Show();
            };
            awayButton.clicked += () => {
                betApp.ctx.paymentApp.SetWinBet(currentMatchInfo, oddAway, currentMatchInfo.Away);
                betApp.ctx.paymentApp.Show();
            };
            drawButton.clicked += () => {
                betApp.ctx.paymentApp.SetWinBet(currentMatchInfo, oddDraw, null);
                betApp.ctx.paymentApp.Show();
            };
        }
        public void SetMatchInfo(MatchInfo newMatch)
        {
            if(currentMatchInfo != null)
                currentMatchInfo.MatchStateChange -= UpdateOnStateChange;

            currentMatchInfo = newMatch;
            currentMatchInfo.MatchStateChange += UpdateOnStateChange;
            (oddHome, oddAway, oddDraw) = BetManager.CalculateWinOdd(currentMatchInfo);
        }
        void UpdateTeamName()
        {
            homeButton.text = $"{currentMatchInfo.Home.name}\r\n<color=green>{oddHome:F2}x";
            awayButton.text = $"{currentMatchInfo.Away.name}\r\n<color=green>{oddAway:F2}x";
            drawButton.text = $"Draw<color=green>\r\n{oddDraw:F2}x";
            timeDisplay.text = currentMatchInfo.MatchState == MatchState.Waiting ? $"{currentMatchInfo.StartTime.FormatedDay('/')} {currentMatchInfo.StartTime.FormatedTime('h')}" : "Half Time";
        }
        void UpdateOnStateChange(MatchState state)
        {
            if(state == MatchState.FirstHalf || state == MatchState.SecondHalf)
                betApp.ChangeWindow(betApp.matchSelectionWindow);

        }
        public override void Show()
        {
            UpdateTeamName();
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
        }
    }
}