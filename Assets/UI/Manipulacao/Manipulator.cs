using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Bet.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ManipulatorUI : MonoBehaviour
{
    [SerializeField] UIDocument document;
    ScrollView matchScrollView;
    public VisualElement root, matchSelection, manipulacaoSelection;
    List<MatchButton> matchButtons = new();
    MatchInfo currentMatch;
    Button close;
    Button homeYellow, homeRed, homeOwnGoal, homeThrow;
    Button awayYellow, awayRed, awayOwnGoal, awayThrow;
    int currentDay = 0;
    Interactable interactable;
    void ChangeCurrentMatch(MatchInfo info)
    {
        if(currentMatch != null) currentMatch.MatchStateChange -= ReturnOnState;
        currentMatch = info;
        currentMatch.MatchStateChange += ReturnOnState;
    }
    void ReturnOnState(MatchState state)
    {
        if(state != MatchState.MatchEnded) return;
        ReturnToMatchSelection();
    }
    void ReturnToMatchSelection()
    {
        matchSelection.Display(true);
        manipulacaoSelection.Display(false);
        close.clicked -= ReturnToMatchSelection;
        close.clicked += CloseManipulator;
    }
    void OnEnable()
    {
        root = document.rootVisualElement.Q<VisualElement>("Manipulacao");
        matchSelection = root.Q<VisualElement>("MatchSelection");
        manipulacaoSelection = root.Q<VisualElement>("ManipulacaoSelection");
        close = root.Q<Button>("Close");

        matchScrollView = matchSelection.Q<ScrollView>("ScrollView");

        homeYellow = manipulacaoSelection.Q<Button>("HomeYellow");
        homeRed = manipulacaoSelection.Q<Button>("HomeRed");
        homeOwnGoal = manipulacaoSelection.Q<Button>("HomeOwnGoal");
        homeThrow = manipulacaoSelection.Q<Button>("HomeThrow");
        awayYellow = manipulacaoSelection.Q<Button>("AwayYellow");
        awayRed = manipulacaoSelection.Q<Button>("AwayRed");
        awayOwnGoal = manipulacaoSelection.Q<Button>("AwayOwnGoal");
        awayThrow = manipulacaoSelection.Q<Button>("AwayThrow");

        matchSelection.Display(true);
        manipulacaoSelection.Display(false);
        MatchManager.CreatedMatch += OnCreatedMatch;
        ClockManager.DayPassed += d => {currentDay = d; UpdateMatches();};
        close.clicked += CloseManipulator;

        homeYellow.clicked += () => ManipulateCard(FoulIntensity.Tatical, "home");
        awayYellow.clicked += () => ManipulateCard(FoulIntensity.Tatical, "away");
        homeRed.clicked += () => ManipulateCard(FoulIntensity.Brutal, "home");
        awayRed.clicked += () => ManipulateCard(FoulIntensity.Brutal, "away");
        homeOwnGoal.clicked += () => ManipulateOwnGoal("home");
        awayOwnGoal.clicked += () => ManipulateOwnGoal("away");
        homeThrow.clicked += () => ManipulateThrow("home");
        awayThrow.clicked += () => ManipulateThrow("away");

        root.Display(false);
    }
    void StopManipulation()
    {
        matchButtons.ForEach(b =>
        {
            if(b.info == currentMatch)
                b.button.Display(false);

            ReturnToMatchSelection();
        });
    }
    void ManipulateThrow(string team)
    {
        if(currentMatch.MatchState != MatchState.FirstHalf && currentMatch.MatchState != MatchState.SecondHalf) return;
        if(team == "home")
            currentMatch.throwingTeam = currentMatch.Home;
        if(team == "away")
            currentMatch.throwingTeam = currentMatch.Away;

        StopManipulation();
    }
    void ManipulateCard(FoulIntensity foulIntensity, string team)
    {
        if(currentMatch.MatchState != MatchState.FirstHalf && currentMatch.MatchState != MatchState.SecondHalf) return;
        if(team == "home")
            currentMatch.CommitFoul(foulIntensity, Player.GetRandomPlayer(currentMatch.activePlayers.Where(p => p.team == currentMatch.Home).ToList()));
        else
            currentMatch.CommitFoul(foulIntensity, Player.GetRandomPlayer(currentMatch.activePlayers.Where(p => p.team == currentMatch.Away).ToList()));
        StopManipulation();
    }
    void ManipulateOwnGoal(string team)
    {
        if(currentMatch.MatchState != MatchState.FirstHalf && currentMatch.MatchState != MatchState.SecondHalf) return;
        if(team == "home")
            currentMatch.RandomOwnGoal(currentMatch.Home);
        if(team == "away")
            currentMatch.RandomOwnGoal(currentMatch.Away);
        StopManipulation();
    }
    void UpdateMatches()
    {
        matchScrollView.Clear();
        matchButtons.Sort((a, b) => a.info.StartTime.CompareTo(b.info.StartTime));
        matchButtons.ForEach(m =>{
            if(m.info.StartTime.Day != currentDay) return;
            matchScrollView.Add(m.button);
        });
    }
    public void GoToManipulation(MatchInfo info)
    {
        ChangeCurrentMatch(info);

        homeYellow.text = info.Home.name;
        homeRed.text = info.Home.name;
        homeOwnGoal.text = info.Home.name;
        homeThrow.text = info.Home.name;
        awayYellow.text = info.Away.name;
        awayRed.text = info.Away.name;
        awayOwnGoal.text = info.Away.name;
        awayThrow.text = info.Away.name;

        matchSelection.Display(false);
        manipulacaoSelection.Display(true);

        close.clicked += ReturnToMatchSelection;
        close.clicked -= CloseManipulator;
    }

    public void CloseManipulator() {root.Display(false); interactable.DeInteract();}
    public void OpenManipulator(Interactable interactable) {root.Display(true); this.interactable = interactable;}
    void OnCreatedMatch(MatchInfo info)
    {
        UpdateMatches();
        var matchButton = new MatchButton(this, info);
        matchButtons.Add(matchButton);
    }
}

public class MatchButton
{
    public MatchInfo info;
    public Button button;
    public MatchButton(ManipulatorUI manipulator, MatchInfo info)
    {
        this.info = info;
        button = new Button
        {
            text = $"{info.Home.name} x {info.Away.name}"
        };
        button.AddToClassList("button");

        info.MatchStateChange += state => button.Display(state switch
        {
            MatchState.MatchEnded => false,
            _ => true
        });

        button.Display(false);
        button.clicked += () => manipulator.GoToManipulation(info);
    }
}
