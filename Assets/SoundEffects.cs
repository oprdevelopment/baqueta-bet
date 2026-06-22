using Assets.Bet;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioClip[] clips;
    void Start()
    {
        Bet.BetPlaced += Bettts;
        PlayClip(clips[4]);
    }
    void Bettts(Bet b)
    {
        b.StateChanged += state =>
        {
            if(state == BetState.Won)   
                PlayClip(clips[3]);
        };
    }
    public void PlayClip(AudioClip clip)
    {
        var a = new GameObject(clip.name);
        var b = a.AddComponent<AudioSource>();
        b.clip = clip;
        b.Play();
    }
}
