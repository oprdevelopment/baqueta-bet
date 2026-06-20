using UnityEngine;

namespace Assets.Interaction
{    
public class LockToScreen : Interactable
{
    PlayerMovement playerMovement;
    [SerializeField] Transform lockPosition;
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

        public override void Interact()
        {
            base.Interact();
            playerMovement.LockCam(true);
            playerMovement.StartCoroutine(playerMovement.LerpCam(lockPosition));
        }
}
}
