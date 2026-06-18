using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class MatchCard : IVisualElementDisplay
    {
        public TemplateContainer Card {get; private set;}
        BetUiManager BetUiManager;
        Window betWindow;
        MatchInfo matchInfo;
        VisualElement stateContainer, dayTimeContainer;
        Label teamNameDisplayer, stateDisplayer, scoreDisplayer, hourDisplayer, dayDisplayer;
        Button betButton;
        public MatchCard(BetUiManager betUiManager, Window betWindow, VisualTreeAsset matchCardTemplate, MatchInfo matchInfo)
        {
            this.Card = matchCardTemplate.Instantiate();
            this.matchInfo = matchInfo;
            this.BetUiManager = betUiManager;
            this.betWindow = betWindow;

            dayTimeContainer = Card.Q<VisualElement>("DayTimeContainer");
            stateContainer = Card.Q<VisualElement>("StateContainer");
            teamNameDisplayer = Card.Q<Label>("Team");
            stateDisplayer = Card.Q<Label>("GameState");
            scoreDisplayer = Card.Q<Label>("Score");
            hourDisplayer = Card.Q<Label>("Hour");
            dayDisplayer = Card.Q<Label>("Day");
            betButton = Card.Q<Button>("BetButton");

            hourDisplayer.text = matchInfo.StartTime.FormatedTime('h');
            dayDisplayer.text = matchInfo.StartTime.FormatedDay('/');
            UpdateTeamName();
            Card.Display(false);
        }
        void GoToBetWindow()
        {
            BetUiManager.currentMatchInfo = matchInfo;
            BetUiManager.ChangeWindow(betWindow);
        }
        void UpdateMatchState(MatchState matchState)
        {
            bool Waiting = matchState == MatchState.Waiting;
            bool Playing = matchState == MatchState.FirstHalf || matchState == MatchState.SecondHalf;
            bool Endend = matchState == MatchState.MatchEnded;
            
            scoreDisplayer.Display(!Waiting);
            betButton.SetEnabled(!Playing);
            dayTimeContainer.Display(Waiting);
            stateContainer.Display(!Waiting);
            
            if(Endend) Hide();
        }
        void UpdateTime()
        {
            if(matchInfo.MatchState == MatchState.Waiting || matchInfo.MatchState == MatchState.MatchEnded) return;

            stateDisplayer.text = matchInfo.MatchState switch
            {
                MatchState.FirstHalf => $"{matchInfo.GameTime.Formated()}'",
                MatchState.Interval => $"Half\r\nTime",
                MatchState.SecondHalf => $"{(matchInfo.GameTime + 45).Formated()}'",
                _ => ""
            };
        }
        void UpdateScore() => scoreDisplayer.text = $"{matchInfo.HomeScore}\r\n{matchInfo.AwayScore}";
        void UpdateTeamName() => teamNameDisplayer.text = $"{matchInfo.Home.name}\r\n{matchInfo.Away.name}";
        public void Show()
        {
            UpdateMatchState(matchInfo.MatchState);
            UpdateScore();
            UpdateTime();

            Card.Display(true);
            ClockManager.Tick += UpdateTime;
            matchInfo.MatchStateChange += UpdateMatchState;
            matchInfo.GoalScored += UpdateScore;
            betButton.clicked += GoToBetWindow;
        }
        public void Hide()
        {
            Card.Display(false);
            ClockManager.Tick -= UpdateTime;
            matchInfo.MatchStateChange -= UpdateMatchState;
            matchInfo.GoalScored -= UpdateScore;
            betButton.clicked -= GoToBetWindow;
        }
    }
}