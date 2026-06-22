using UnityEngine.UIElements;
using System.Collections.Generic;
using Assets.Bet.Bets;
using UnityEngine;
using Unity.Mathematics;

namespace Assets.Bet.UI
{

    public abstract class Window : IVisualElementDisplay
    {
        public VisualElement window;
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
        int currentDay = 0;
        public MatchSelectionWindow(BetApp app, VisualElement windowContainer) : base(app, windowContainer)
        {
            this.betApp = app;
            matchScrollView = windowContainer.Q<ScrollView>("MatchScrollView");

            MatchManager.CreatedMatch += OnCreatedMatch;
            ClockManager.DayPassed += info => 
            {
                currentDay = info;
                matchCards.ForEach(m =>
                {
                    if(m.matchInfo.StartTime.Day != currentDay) m.Hide();
                    else m.Show(); 
                });
            };
        }
        public void UpdateCards()
        {
            matchScrollView.Clear();
            matchCards.Sort((a, b) => a.matchInfo.StartTime.CompareTo(b.matchInfo.StartTime));
            matchCards.ForEach(c => {
                matchScrollView.Add(c.Card);
            });        
        }
        void OnCreatedMatch(MatchInfo matchInfo)
        {
            MatchCard newMatchCard = new(betApp, matchInfo);
            matchCards.Add(newMatchCard);
            if(matchInfo.StartTime.Day != currentDay) newMatchCard.Hide();
            UpdateCards();
        }
    }
    public class BetVisualizationWindow : Window
    {
        public List<BetCard> betCards = new();
        BetApp betApp;
        VisualElement betScrollView;
        public BetVisualizationWindow(BetApp app, VisualElement windowContainer) : base(app, windowContainer)
        {
            this.betApp = app;
            betScrollView = windowContainer.Q<ScrollView>("BetScrollView");

            Bet.BetPlaced += OnCreatedBet;
            betScrollView.Clear();
        }
        public void UpdateCards()
        {
            betScrollView.Clear();
            betCards.Sort((a, b) => a.betInfo.betTime.CompareTo(b.betInfo.betTime));
            betCards.ForEach(c => {
                betScrollView.Insert(0, c.Card);
                c.Show();
            });        
        }
        void OnCreatedBet(Bet betInfo)
        {
            Debug.Log("InsertBet");
            BetCard newBetCard = new(betApp, betInfo);
            betCards.Add(newBetCard);
            UpdateCards();
        }
    }
    public class BetMakerWindow : Window
    {
        Button homeButton, awayButton, drawButton, betYellow, betRed;
        Label timeDisplay, oddYellowLabel, oddRedLabel, countYellowLabel, countRedLabel;
        DropdownField dropdownYellow, dropdownRed;
        SliderInt sliderYellow, sliderRed;
        BetApp betApp;
        public VisualElement BetMakerContainer;
        public List<MatchCard> matchCards = new();
        float oddHome, oddAway, oddDraw;
        float oddYellow, oddRed;
        MatchInfo currentMatchInfo;
        int countYellow, countRed;
        ComparisonType comparisonYellow = ComparisonType.Over, comparisonRed = ComparisonType.Over;

        void UpdateCount(CardType color, int count)
        {
            if(color == CardType.Yellow)
            {
                countYellow = count;
                countYellowLabel.text = (count + .5f).ToString();
            }
            if(color == CardType.Red)
            {
                countRed = count;
                countRedLabel.text = (count + .5f).ToString();
            }
        }
        public BetMakerWindow(BetApp betApp, VisualElement window) : base(betApp, window)
        {
            this.betApp = betApp;

            homeButton = window.Q<Button>("HomeWinButton");
            awayButton = window.Q<Button>("AwayWinButton");
            drawButton = window.Q<Button>("DrawButton");
            timeDisplay = window.Q<Label>("Time");

            betYellow = window.Q<Button>("BetYellow");
            oddYellowLabel = window.Q<Label>("OddYellow");
            countYellowLabel = window.Q<Label>("CountYellow");
            dropdownYellow = window.Q<DropdownField>("DropdownYellow");
            sliderYellow = window.Q<SliderInt>("SliderYellow");

            betRed = window.Q<Button>("BetRed");
            oddRedLabel = window.Q<Label>("OddRed");
            countRedLabel = window.Q<Label>("CountRed");
            dropdownRed = window.Q<DropdownField>("DropdownRed");
            sliderRed = window.Q<SliderInt>("SliderRed");

            dropdownYellow.RegisterCallback<ChangeEvent<string>>(c => 
            {
                comparisonYellow = c.newValue switch
                {
                    "Over" => ComparisonType.Over,
                    "Under" => ComparisonType.Under,
                };
                if(comparisonYellow == ComparisonType.Under) sliderYellow.lowValue = 1;
                else sliderYellow.lowValue = 0;
                
                UpdateCount(CardType.Yellow, countYellow);
            });
            dropdownRed.RegisterCallback<ChangeEvent<string>>(c => 
            {
                comparisonRed = c.newValue switch
                {
                    "Over" => ComparisonType.Over,
                    "Under" => ComparisonType.Under,
                };
                if(comparisonRed == ComparisonType.Under) sliderRed.lowValue = 1;
                else sliderRed.lowValue = 0;

                UpdateCount(CardType.Red, countRed);
            });

            sliderYellow.RegisterValueChangedCallback(c =>
            {
                UpdateCount(CardType.Yellow, c.newValue);
                (float oddGreater, float oddLess) = BetManager.CalculateOddCard(currentMatchInfo, CardType.Yellow, countYellow);
                oddYellow = comparisonYellow switch
                {
                    ComparisonType.Over => oddGreater,
                    ComparisonType.Under => oddLess,
                };     
                oddYellowLabel.text = oddYellow.ToString();        
            });

            betYellow.clicked += () =>
            {
                betApp.ctx.paymentApp.SetCardBet(CardType.Yellow, oddYellow, currentMatchInfo, comparisonYellow, countYellow + 0.5f);
                betApp.ctx.paymentApp.Show();
            };

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

            sliderYellow.value = 5;
            dropdownYellow.value = "Over";
            sliderRed.value = 5;
            dropdownRed.value = "Over";
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