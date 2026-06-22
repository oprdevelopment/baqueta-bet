using System.Collections.Generic;
using Assets.Bet.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UIElements;

public class TELEVISAO : MonoBehaviour
{
    [SerializeField] UIDocument document, controllerDocument;
    public Button quitButton;
    public VisualElement root, controllerRoot;
    public VisualTreeAsset matchTreeAsset, infoTreeAsset;
    public ScrollView controllerScrollView;
    public int currentDay = 0;
    Interactable interactable;
    List<MatchVisu> matchVisus = new();
    void OnEnable()
    {
        root = document.rootVisualElement;
        controllerRoot = controllerDocument.rootVisualElement.Q<VisualElement>("TVSelection");
        
        controllerScrollView = controllerRoot.Q<ScrollView>("ScrollView");
        quitButton = controllerRoot.Q<Button>("Quit");

        MatchManager.CreatedMatch += OnCreatedMatch;
        ClockManager.DayPassed += info =>
        {
            currentDay = info;
            matchVisus.ForEach(m =>
            {
                m.butom.Display(false);
                if(m.info.MatchState == MatchState.MatchEnded || m.info.StartTime.Day != currentDay) return;
                m.butom.Display(true);
            });
        };

        quitButton.clicked += HideController;

        controllerRoot.Display(false);
    }
    public void ShowController(Interactable interactable)
    {
        this.interactable = interactable;
        controllerRoot.pickingMode = PickingMode.Position;
        controllerRoot.Display(true);
    }
    public void HideController()
    {
        controllerRoot.Display(false);
        controllerRoot.pickingMode = PickingMode.Ignore;
        interactable.DeInteract();
    }
    public void UpdateCards()
    {
        controllerScrollView.Clear();
        matchVisus.Sort((a, b) => a.info.StartTime.CompareTo(b.info.StartTime));
        matchVisus.ForEach(c => {
            controllerScrollView.Add(c.butom);
        });        
    }
    public void EnableMatchVisu(MatchVisu visu)
    {
        matchVisus.ForEach(v =>
        {
            if(v != visu) v.visu.Display(false);
        });

        visu.visu.Display(true);
    }
    void OnCreatedMatch(MatchInfo info)
    {
        var visu = new MatchVisu(this, info);
        matchVisus.Add(visu);
        UpdateCards();
    }
}

public class MatchVisu
{
    public MatchInfo info;
    public TELEVISAO tv;
    public TemplateContainer visu;
    public Button butom;
    public Label score, time;
    ScrollView scrollView;
    public MatchVisu(TELEVISAO tv, MatchInfo info)
    {
        this.info = info;
        this.tv = tv;
        visu = tv.matchTreeAsset.Instantiate();
        visu.Q<Label>("Home").text = info.Home.name;
        visu.Q<Label>("Away").text = info.Away.name;

        scrollView = visu.Q<ScrollView>("ScrollView");   
        score = visu.Q<Label>("Score");
        time = visu.Q<Label>("Time");

        butom = new Button(){text = $"{info.Home.name} X {info.Away.name}"};
        butom.AddToClassList("tv-button");

        tv.controllerScrollView.Add(butom);
        if(info.StartTime.Day != tv.currentDay) butom.Display(false);

        ClockManager.Tick += () =>
        {
            if(info.MatchState != MatchState.FirstHalf && info.MatchState != MatchState.SecondHalf) return;
            time.text = $"{info.GetRealGameTime()}'";  
        };

        info.GoalScoredInfo += (team, ownGoal) =>
        {
            score.text = $"{this.info.HomeScore}-{this.info.AwayScore}";
            var infoAsset = tv.infoTreeAsset.Instantiate();

            if (!ownGoal)
            {
                infoAsset.Q<Label>("HomeInfo").text = team == info.Home ? "Goal" : "";
                infoAsset.Q<Label>("AwayInfo").text = team == info.Away ? "Goal" : "";
            }
            else
            {
                infoAsset.Q<Label>("HomeInfo").text = team != info.Home ? "Goal (Own Goal)" : "";
                infoAsset.Q<Label>("AwayInfo").text = team != info.Away ? "Goal (Own Goal)" : "";
            }

            infoAsset.Q<Label>("Time").text = $"{info.GetRealGameTime()}'";

            scrollView.Insert(0, infoAsset);
        };

        info.FoulCommited += player =>
        {
            var infoAsset = tv.infoTreeAsset.Instantiate();
            infoAsset.Q<Label>("HomeInfo").text = player.team != info.Home ? "Foul" : "";
            infoAsset.Q<Label>("AwayInfo").text = player.team != info.Away ? "Foul" : "";
            infoAsset.Q<Label>("Time").text = $"{info.GetRealGameTime()}'";
            scrollView.Insert(0, infoAsset);
        };
        info.CardGiven += (cardType, player) =>
        {
            var infoAsset = tv.infoTreeAsset.Instantiate();
            infoAsset.Q<Label>("HomeInfo").text = player.team != info.Home ? $"{cardType} to {player.name}" : "";
            infoAsset.Q<Label>("AwayInfo").text = player.team != info.Away ? $"{cardType} to {player.name}" : "";
            infoAsset.Q<Label>("Time").text = $"{info.GetRealGameTime()}'";
            scrollView.Insert(0, infoAsset);
        };

        score.text = "Waiting";
        time.text = $"{info.StartTime.FormatedTime('h')}";

        info.MatchStateChange += state =>
        {
            if(state == MatchState.FirstHalf) score.text = "0-0";
            time.text = state switch
            {
                MatchState.Waiting => "Waiting",
                MatchState.Interval => "Half Time",
                MatchState.MatchEnded => "Ended",
                _ =>  $"{info.GetRealGameTime()}'"
            };
        };

        butom.clicked += () => tv.EnableMatchVisu(this);

        visu.Display(false);
        tv.root.Add(visu);
    }
}
