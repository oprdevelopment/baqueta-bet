using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class IntExtension
{
    public static string Formated(this int number)
    {
        return number < 10 ? $"0{number}" : number.ToString(); 
    }
}

public class BetUiManager : MonoBehaviour
{
    [SerializeField] VisualTreeAsset matchListItemTemplate;
    UIDocument document;
    VisualElement root, matchScrollView;
    Label clock;
    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        matchScrollView = root.Q<VisualElement>("MatchScrollView");
        clock = root.Q<Label>("Clock");

        MatchManager.CreatedMatch += OnCreatedMatch;
        ClockManager.TickInfo += OnTickInfo;
    }

    void OnDisable()
    {
        MatchManager.CreatedMatch -= OnCreatedMatch;
        ClockManager.TickInfo -= OnTickInfo;
    }

    void OnTickInfo(TimeInfo timeInfo)
    {
        var hourString = timeInfo.Hours.Formated();
        var minuteString = timeInfo.Minutes.Formated();
        var dayString = (ClockManager.StartingDay + timeInfo.Day).Formated();
        var monthString = ClockManager.StartingMonth.Formated();

        clock.text = $"{monthString}/{dayString} {hourString}:{minuteString}";
    }

    void OnCreatedMatch(MatchInfo matchInfo)
    {
        var newItem = matchListItemTemplate.Instantiate();
        newItem.Q<Label>("Team").text = $"{matchInfo.Home.name}\r\n{matchInfo.Away.name}";
        UpdateOnMatchState(newItem, matchInfo);
        matchScrollView.Add(newItem);        

        var stateDisplayer = newItem.Q<Label>("GameState");
        var scoreDisplayer = newItem.Q<Label>("Score");

        matchInfo.MatchStateChange += ctx => UpdateOnMatchState(newItem, matchInfo);

        ClockManager.Tick += () =>
        {
            if(matchInfo.MatchState == MatchState.Waiting || matchInfo.MatchState == MatchState.MatchEnded)
                return;
            
            stateDisplayer.text = matchInfo.MatchState switch
            {
                MatchState.FirstHalf => $"{matchInfo.GameTime.Formated()}'",
                MatchState.Interval => $"Half\r\nTime",
                MatchState.SecondHalf => $"{(matchInfo.GameTime + 45).Formated()}'",
                _ => ""
            };
        };

        matchInfo.GoalScored += ctx => {
            scoreDisplayer.text = $"{matchInfo.HomeScore}\r\n{matchInfo.AwayScore}";
        };
    }

    void UpdateOnMatchState(TemplateContainer item, MatchInfo info)
    {
        var timeDisplay = item.Q<VisualElement>("TimeDisplay");
        var stateDisplay = item.Q<VisualElement>("StateDisplay");

        if(info.MatchState != MatchState.Waiting)
        {   
            item.Q<Label>("Score").style.display = DisplayStyle.Flex;
        }

        if(info.MatchState == MatchState.Waiting || info.MatchState == MatchState.Interval)
        {
            item.Q<Button>("BetButton").SetEnabled(true);
        }
        else
        {
            item.Q<Button>("BetButton").SetEnabled(false);
        }

        if(info.MatchState == MatchState.Waiting)
        {
            timeDisplay.style.display = DisplayStyle.Flex;
            stateDisplay.style.display = DisplayStyle.None;

            item.Q<Label>("Hour").text = $"{info.StartTime.Hours.Formated()}:{info.StartTime.Minutes.Formated()}";
            item.Q<Label>("Day").text = $"{ClockManager.StartingMonth.Formated()}/{(ClockManager.StartingDay + info.StartTime.Day).Formated()}";
        }
        else if(info.MatchState == MatchState.MatchEnded)
        {
            item.style.display = DisplayStyle.None;
        }
        else
        {
            timeDisplay.style.display = DisplayStyle.None;
            stateDisplay.style.display = DisplayStyle.Flex;
        }
    }
}
