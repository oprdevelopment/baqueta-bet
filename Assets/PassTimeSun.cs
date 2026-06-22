using System;
using UnityEngine;

public class PassTimeSun : MonoBehaviour
{
    Light light;
    [SerializeField] Renderer emissive;
    void Awake()
    {
        light = GetComponent<Light>();
    }
    void OnEnable()
    {
        ClockManager.TickInfo += OnTickInfo;
    }
    void OnDisable()
    {
        ClockManager.TickInfo -= OnTickInfo;
    }

    void OnTickInfo(TimeInfo timeInfo)
    {
        float time = timeInfo.Hours + (timeInfo.Minutes / 60f);
        
        float t = time / 24;

        t = Mathf.Sin(t * Mathf.PI);

        Color morning = new Color(1f, 0.6f, 0.3f);
        Color noon = Color.white;
        Color evening = new Color(1f, 0.25f, 0.15f);

        Color c = Color.Lerp(morning, noon, t);
        c = Color.Lerp(c, evening, Mathf.Clamp01(time / 18f));

        float angle = Mathf.Lerp(-90f, 90f, t);
        float color = Mathf.InverseLerp(12f, 15f, t);

        transform.eulerAngles = new(angle, -90, 0);
        light.color = c;

        light.intensity = Mathf.Lerp(0f, 15f, t);
        if(timeInfo.Hours > 18 || timeInfo.Hours < 4)
            light.intensity = 0;

        float emissionStrength;
        if (timeInfo.Hours < 5f || timeInfo.Hours > 20f)
        {
            emissionStrength = 0f;
        }
        else
        {
            emissionStrength = Mathf.Sin(
                (timeInfo.Hours - 6f) / 12f * Mathf.PI
            );
        }
        emissive.material.SetColor("_EmissionColor", Color.white * emissionStrength * 2.2f);
        
    }
}
