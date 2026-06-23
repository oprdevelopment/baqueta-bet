using UnityEngine;

public class Clockss : MonoBehaviour
{
    [SerializeField] Transform hT, mT;
    void OnEnable()
    {
        ClockManager.TickInfo += Rotate;
    }
    void OnDisable()
    {
        ClockManager.TickInfo -= Rotate;
    }
    void Rotate(TimeInfo info)
    {
        float pHour = (info.Hours % 12 + info.Minutes / 60f) / 12f;
        float pMin = info.Minutes / 60.0f;

        print(pHour);

        hT.localEulerAngles = new(pHour * 360, 0, 0);
        mT.localEulerAngles = new(pMin * 360, 0, 180);
    }
}
