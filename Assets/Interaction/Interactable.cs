using UnityEngine;
[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    Outline outline;
    bool canEnable = true;
    void Start()
    {
        outline = GetComponent<Outline>();
        outline. enabled = false;
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
        if(canEnable && Input.GetMouseButtonDown(0))
            Interact();
    }

}
