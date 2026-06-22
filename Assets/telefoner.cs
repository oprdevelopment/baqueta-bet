using Assets.Interaction;
using UnityEngine;

public class telefoner : MonoBehaviour
{
    ManipulateGame manipul;
    void Awake()
    {
        manipul = GetComponent<ManipulateGame>();
        manipul.enabled = false;        
    }
    void Ligar(TimeInfo info)
    {
        if(info.Day == 1 && info.Hours == 12)
        {
            manipul.enabled = true;
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
