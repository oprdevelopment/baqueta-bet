using Assets.Bet.UI;
using UnityEngine;

namespace Assets.Interaction
{
public class ManipulateGame : Interactable
{
    [SerializeField] ManipulatorUI manipulatorUI;

        public override void Interact()
        {
            base.Interact();
            manipulatorUI.OpenManipulator(this);
            playerMovement.LockCam(true);
            Cursor.visible = true;
        }

        public override void DeInteract()
        {
            base.DeInteract();
            playerMovement.LockCam(false);
            Cursor.visible = false;
        }
}
}