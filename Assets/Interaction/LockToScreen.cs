using UnityEngine;

namespace Assets.Interaction
{    
public class LockToScreen : Interactable
{
    protected PlayerMovement playerMovement;
    [SerializeField] Transform lockPosition;
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

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
