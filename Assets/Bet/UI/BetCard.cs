using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Bet.UI
{
    public class BetCard : IVisualElementDisplay
    {
        public TemplateContainer Card {get; private set;}
        BetApp BetApp;
        public Bet betInfo;
        Label teamNameDisplayer, infoDisplayer, betTypeDisplayer, betStateDisplayer;
        public BetCard(BetApp betApp, Bet betInfo)
        {
            this.Card = betApp.ctx.betCardTemplate.Instantiate();
            this.betInfo = betInfo;
            this.BetApp = betApp;

            teamNameDisplayer = Card.Q<Label>("Team");
            infoDisplayer = Card.Q<Label>("Info");
            betTypeDisplayer = Card.Q<Label>("Type");
            betStateDisplayer = Card.Q<Label>("State");

            teamNameDisplayer.text = $"{betInfo.matchInfo.Home.name} x {betInfo.matchInfo.Away.name}";
            betTypeDisplayer.text = betInfo.GetBetType();
            infoDisplayer.text = $"${betInfo.Amount:F2}\r\n<color=green>{betInfo.Multiplier}x";
            betStateDisplayer.text = $"In Progress\r\n${betInfo.Amount * betInfo.Multiplier:F2}";

            betInfo.StateChanged += ctx => {
                if(ctx != BetState.Lost && ctx != BetState.Won) return;
                betStateDisplayer.text = ctx switch
                {
                    BetState.Lost => $"<color=red>Lost",  
                    BetState.Won => $"Won <color=green>${betInfo.Amount * betInfo.Multiplier:F2}",
                    _ => ""  
                };
            };
        }
        public void Show()
        {
            Card.Display(true);
        }
        public void Hide()
        {
            Card.Display(false);
        }
    }
}