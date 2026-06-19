using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public static class IntExtension
{
    public static string Formated(this int number) => number < 10 ? $"0{number}" : number.ToString(); 
}

namespace Assets.Bet.UI
{
    public static class UIExtensions
    {
        public static void Display(this VisualElement element, bool show) => element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
    public interface IVisualElementDisplay
    {
        public void Show();
        public void Hide();
    }
    public class BetUiManager : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset matchCardTemplate;
        UIDocument document;
        VisualElement root, matchScrollView;
        public Window matchSelectionWindow, betMakerWindow, currentWindow;
        Label systemClock;
        List<IVisualElementDisplay> matchCards;
        public MatchInfo currentMatchInfo;
        public void ChangeWindow(Window newWindow)
        {
            currentWindow?.Hide();
            newWindow.Show();
            currentWindow = newWindow;
        }
        private void OnEnable()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            matchScrollView = root.Q<VisualElement>("MatchScrollView");
            
            betMakerWindow = new BetMakerWindow(this, root.Q<VisualElement>("BetMakeContainer"));
            matchSelectionWindow = new(this, root.Q<VisualElement>("MatchSelectionContainer"));

            ChangeWindow(matchSelectionWindow);

            systemClock = root.Q<Label>("Clock");
            matchCards = new();

            MatchManager.CreatedMatch += OnCreatedMatch;
            ClockManager.TickInfo += UpdateSystemClock;
        }
        void OnDisable()
        {
            MatchManager.CreatedMatch -= OnCreatedMatch;
            ClockManager.TickInfo -= UpdateSystemClock;
        }

        void OnCreatedMatch(MatchInfo matchInfo)
        {
            MatchCard matchCard = new(this, betMakerWindow, matchCardTemplate, matchInfo);
            matchScrollView.Add(matchCard.Card);
            matchCards.Add(matchCard);
            matchCard.Show();
        }

        void UpdateSystemClock(TimeInfo timeInfo)
        {
            systemClock.text = timeInfo.FormatedDay('/') + " " + timeInfo.FormatedTime(':');
        }
    }
}
