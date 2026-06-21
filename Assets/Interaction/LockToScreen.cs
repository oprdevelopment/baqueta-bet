using UnityEngine;

namespace Assets.Interaction
{    
public class LockToScreen : Interactable
{
    [SerializeField] Transform lockPosition;
        public override void Interact()
        {
            base.Interact();
            playerMovement.StartCoroutine(playerMovement.LerpCam(lockPosition));
        }
        public override void DeInteract()
        {
            base.DeInteract();
            playerMovement.StartCoroutine(playerMovement.LerpCam());
        }
}
}
