using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class MatchCard : IVisualElementDisplay
    {
        public TemplateContainer MatchCardTemplate {get; private set;}
        MatchInfo matchInfo;
        VisualElement stateContainer, dayTimeContainer;
        Label teamNameDisplayer, stateDisplayer, scoreDisplayer, hourDisplayer, dayDisplayer;
        Button betButton;
        public MatchCard(VisualTreeAsset matchCardTemplate, MatchInfo matchInfo)
        {
            this.MatchCardTemplate = matchCardTemplate.Instantiate();
            this.matchInfo = matchInfo;
            
            dayTimeContainer = MatchCardTemplate.Q<VisualElement>("DayTimeContainer");
            stateContainer = MatchCardTemplate.Q<VisualElement>("StateContainer");
            teamNameDisplayer = MatchCardTemplate.Q<Label>("Team");
            stateDisplayer = MatchCardTemplate.Q<Label>("GameState");
            scoreDisplayer = MatchCardTemplate.Q<Label>("Score");
            hourDisplayer = MatchCardTemplate.Q<Label>("Hour");
            dayDisplayer = MatchCardTemplate.Q<Label>("Day");
            betButton = MatchCardTemplate.Q<Button>("BetButton");

            hourDisplayer.text = matchInfo.StartTime.FormatedTime('h');
            dayDisplayer.text = matchInfo.StartTime.FormatedDay('/');
            UpdateTeamName();
        }
        void UpdateMatchState(MatchState matchState)
        {
            bool Waiting = matchState == MatchState.Waiting;
            bool Playing = matchState == MatchState.FirstHalf || matchState == MatchState.SecondHalf;
            bool Endend = matchState == MatchState.MatchEnded;
            
            scoreDisplayer.Show(!Waiting);
            betButton.SetEnabled(!Playing);
            dayTimeContainer.Show(Waiting);
            stateContainer.Show(!Waiting);
            
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

            MatchCardTemplate.Show(true);
            ClockManager.Tick += UpdateTime;
            matchInfo.MatchStateChange += UpdateMatchState;
            matchInfo.GoalScored += UpdateScore;
        }
        public void Hide()
        {
            MatchCardTemplate.Show(false);
            ClockManager.Tick -= UpdateTime;
            matchInfo.MatchStateChange -= UpdateMatchState;
            matchInfo.GoalScored -= UpdateScore;
        }
    }
}