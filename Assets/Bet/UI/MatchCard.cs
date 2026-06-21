using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class MatchCard : IVisualElementDisplay
    {
        public TemplateContainer Card {get; private set;}
        BetApp BetApp;
        public MatchInfo matchInfo;
        VisualElement stateContainer, dayTimeContainer;
        Label teamNameDisplayer, stateDisplayer, scoreDisplayer, hourDisplayer, dayDisplayer;
        public Button betButton;
        public MatchCard(BetApp betApp, MatchInfo matchInfo)
        {
            this.Card = betApp.ctx.matchCardTemplate.Instantiate();
            this.matchInfo = matchInfo;
            this.BetApp = betApp;

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
        }
        void GoToBetWindow()
        {
            BetApp.betMakerWindow.SetMatchInfo(matchInfo);
            BetApp.ChangeWindow(BetApp.betMakerWindow);
        }
        void UpdateMatchState(MatchState matchState)
        {
            bool Waiting = matchState == MatchState.Waiting;
            bool Playing = matchState == MatchState.FirstHalf || matchState == MatchState.SecondHalf;
            bool Ended = matchState == MatchState.MatchEnded;

            scoreDisplayer.Display(!Waiting);
            betButton.SetEnabled(!Ended && !Playing);
            dayTimeContainer.Display(Waiting);
            stateContainer.Display(!Waiting);       
            if(Ended) Hide();     
        }
        void UpdateTime()
        {
            if(matchInfo.MatchState == MatchState.Waiting) return;

            stateDisplayer.text = matchInfo.MatchState switch
            {
                MatchState.FirstHalf => $"{matchInfo.GameTime.Formated()}'",
                MatchState.Interval => $"Half\r\nTime",
                MatchState.SecondHalf => $"{(matchInfo.GameTime + 45).Formated()}'",
                MatchState.MatchEnded => $"Match\r\nEnded",
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

            ClockManager.Tick += UpdateTime;
            matchInfo.MatchStateChange += UpdateMatchState;
            matchInfo.GoalScored += UpdateScore;
            betButton.clicked += GoToBetWindow;

            Card.Display(true);
        }
        public void Hide()
        {
            Card.Display(false);
        }
    }
}