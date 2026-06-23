using Assets.Interaction;
using UnityEngine;

public class telefoner : MonoBehaviour
{
    Interactable manipul;
    void Start()
    {
        manipul = GetComponent<Interactable>();
        manipul.canEnable = false;     
    }
    void Ligar(TimeInfo info)
    {
        if(info.Day == 1 && info.Hours == 12)
        {
            manipul.canEnable = true;
            SoundEffects.Instance.PlayClip(SoundEffects.Instance.clips[10]);
            this.enabled = false;
        }  
    }
    void OnEnable()
    {
        ClockManager.TickInfo += Ligar;
    }
    void OnDisable()
    {
        ClockManager.TickInfo -= Ligar;
    }
}
