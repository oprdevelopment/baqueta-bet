using UnityEngine;
[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    public float MaxDistance = 2;
    Outline outline;
    public bool canEnable = true;
    public bool outlineEnabled = false;
    InputSystem_Actions input;
    protected PlayerMovement playerMovement;
    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        input = new();
        input.Player.Enable();
    }
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }
    public virtual void Interact()
    {
        outline.enabled = false;
        outlineEnabled = false;
        canEnable = false;
    }
    public virtual void DeInteract()
    {
        canEnable = true;
    }
    void OnMouseExit()
    {
        if (outlineEnabled)
        {
            outline.enabled = false;
            outlineEnabled = false;
        }
    }
    void OnMouseOver()
    {
        if(Vector3.Distance(transform.position, playerMovement.transform.position) > MaxDistance){
            if (outlineEnabled)
            {
                outline.enabled = false;
                outlineEnabled = false;
            }
            return;
        }

        if (!outlineEnabled && canEnable)
        {
            outline.enabled = true;
            outlineEnabled = true;
        }

        if(canEnable && input.Player.Interact.WasPressedThisFrame())
            Interact();
    }
}
