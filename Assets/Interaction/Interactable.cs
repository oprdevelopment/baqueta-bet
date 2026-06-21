using UnityEngine;
[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    Outline outline;
    bool canEnable = true;
    InputSystem_Actions input;
    protected PlayerMovement playerMovement;
    void OnEnable()
    {
        input.Player.Enable();
    }
    void OnDisable()
    {
        input.Player.Disable();
    }
    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        input = new();
    }
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }
    public virtual void Interact()
    {
        outline.enabled = false;
        canEnable = false;
    }
    public virtual void DeInteract()
    {
        canEnable = true;
    }

    void OnMouseEnter()
    {
        if(!canEnable) return;
        outline.enabled = true;
    }
    void OnMouseExit()
    {
        if(!canEnable) return;
        outline.enabled = false;
    }
    void OnMouseOver()
    {
        if(canEnable && input.Player.Interact.WasPressedThisFrame())
            Interact();
    }
}
