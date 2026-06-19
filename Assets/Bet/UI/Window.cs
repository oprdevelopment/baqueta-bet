using Assets.Bet.UI;
using UnityEngine.UIElements;
using UnityEngine;

public class Window : IVisualElementDisplay
{
    protected VisualElement window;
    protected BetUiManager betUiManager;
    public Window(BetUiManager betUiManager, VisualElement window)
    {
        this.betUiManager = betUiManager;
        this.window = window;
        window.Display(false);
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

public class BetMakerWindow : Window
{
    Button homeButton, awayButton, drawButton;
    Label timeDisplay;
    public BetMakerWindow(BetUiManager betUiManager, VisualElement window) : base(betUiManager, window)
    {
        homeButton = window.Q<Button>("Home");
        awayButton = window.Q<Button>("Away");
        drawButton = window.Q<Button>("Draw");
        timeDisplay = window.Q<Label>("Time");
    }
    void UpdateTeamName()
    {
        (float oddHome, float oddAway, float oddDraw) = BetManager.CalculateWinOdd(betUiManager.currentMatchInfo);
        homeButton.text = $"{betUiManager.currentMatchInfo.Home.name} <color=green>{oddHome}x";
        awayButton.text = $"{betUiManager.currentMatchInfo.Away.name} <color=green>{oddAway}x";
        drawButton.text = $"Draw <color=green>{oddDraw}x";
        timeDisplay.text = betUiManager.currentMatchInfo.MatchState == MatchState.Waiting ? $"{betUiManager.currentMatchInfo.StartTime.FormatedDay('/')} {betUiManager.currentMatchInfo.StartTime.FormatedTime('h')}" : "Half Time";
    }

    void UpdateOnStateChange(MatchState state)
    {
        if(state == MatchState.FirstHalf || state == MatchState.SecondHalf)
            betUiManager.ChangeWindow(betUiManager.matchSelectionWindow);

    }

    public override void Show()
    {
        base.Show();
        UpdateTeamName();
        betUiManager.currentMatchInfo.MatchStateChange += UpdateOnStateChange;
    }
}