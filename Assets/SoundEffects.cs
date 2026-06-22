using Assets.Bet;
using Unity.Collections;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public static SoundEffects Instance;
    public AudioClip[] clips;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }
    void Start()
    {
        Bet.BetPlaced += Bettts;
    }
    void Bettts(Bet b)
    {
        PlayClip(clips[6]);
        b.StateChanged += state =>
        {
            if(state == BetState.Won)   
                PlayClip(clips[3]);
            if(state == BetState.Lost)
                PlayClip(clips[7]);
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
