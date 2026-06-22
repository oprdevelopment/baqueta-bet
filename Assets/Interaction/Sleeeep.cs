using System;
using System.Collections;
using Assets.Bet.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Sleeeep : Interactable
{
    public UIDocument document;
    public VisualElement bg, root;
    Label clock;
    Button wakeUp;
    ClockManager clockManager;
    public float sleepTransitionTime, sleepSpeed, startingSpeed;
    void OnEnable()
    {
        clockManager = FindAnyObjectByType<ClockManager>();
        startingSpeed = clockManager.secondsPerMinute;

        root = document.rootVisualElement.Q<VisualElement>("Sleep");
        clock = root.Q<Label>("Clock");
        wakeUp = root.Q<Button>("WakeUp");
        
        root.Display(true);
        root.style.opacity = 1;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        wakeUp.clicked += DeInteract;
        ClockManager.TickInfo += info =>
        {
            clock.text = $"{info.FormatedDay('/')} {info.FormatedTime('h')}";  
        };
    }
    IEnumerator Sleep()
    {
        float elapsedTime = 0;
        root.Display(true);

        while(elapsedTime < sleepTransitionTime)
        {
            float percentage = elapsedTime / sleepTransitionTime;
            clockManager.secondsPerMinute = Mathf.Lerp(startingSpeed, sleepSpeed, percentage);
            root.style.opacity = percentage;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        clockManager.secondsPerMinute = sleepSpeed;
        root.style.opacity = 1;
    }
    IEnumerator WakeUp()
    {
        float elapsedTime = 0;

        while(elapsedTime < sleepTransitionTime)
        {
            float percentage = elapsedTime / sleepTransitionTime;
            clockManager.secondsPerMinute = Mathf.Lerp(sleepSpeed, startingSpeed, percentage);
            root.style.opacity = 1 - percentage;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        clockManager.secondsPerMinute = startingSpeed;
        root.style.opacity = 0;
        root.Display(false);
    }
    public override void Interact()
    {
        base.Interact();
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        StartCoroutine(Sleep());
    }
    public override void DeInteract()
    {
        base.DeInteract();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        StartCoroutine(WakeUp());
    }
}
