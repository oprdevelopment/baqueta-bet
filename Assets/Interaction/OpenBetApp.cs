using Assets.Bet.UI;
using Assets.Interaction;
using UnityEngine;

namespace Assets.Interaction
{
public class OpenBetApp : LockToScreen
{
    [SerializeField] BetUiManager betUiManager;
        public override void Interact()
        {
            base.Interact();
            betUiManager.betApp.Show();
        }
}
}