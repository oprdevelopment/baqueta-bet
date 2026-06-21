using System;
using UnityEngine;

[Serializable]
public struct TimeInfo : IComparable<TimeInfo>
{
    public int Hours;
    public int Minutes;
    public int Day;
    public readonly string FormatedTime(char separator) => $"{Hours.Formated()}{separator}{Minutes.Formated()}";
    public readonly string FormatedDay(char separator) => $"{ClockManager.StartingMonth.Formated()}{separator}{(ClockManager.StartingDay + Day).Formated()}";

    public static bool operator ==(TimeInfo left, TimeInfo right)
    {
        return left.Hours == right.Hours && left.Day == right.Day && right.Minutes == left.Minutes;
    }
    public static bool operator !=(TimeInfo left, TimeInfo right)
    {
        return !(left == right);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public int CompareTo(TimeInfo other)
    {
        if(other == null) return 1;

        int day = this.Day.CompareTo(other.Day);
        if (day != 0) return day;

        int hour = this.Hours.CompareTo(other.Hours);
        if (hour != 0) return hour;

        return this.Minutes.CompareTo(other.Minutes);
    }
}

public class ClockManager : MonoBehaviour
{
    public static event Action<TimeInfo> TickInfo;
    public static event Action Tick;
    public static event Action<int> DayPassed;
    [SerializeField] float secondsPerMinute;
    [SerializeField] TimeInfo timeInfo;
    public static int StartingDay {get; private set;} = 16;
    public static int StartingMonth {get; private set;} = 6;
    float elapsedTime;
    void Start()
    {
        timeInfo = new TimeInfo
        {
            Hours = 8,
            Minutes = 0,
            Day = 0
        };

        Tick?.Invoke();
        TickInfo?.Invoke(timeInfo);
    }
    void Update()
    {
        if(elapsedTime * 60 / secondsPerMinute >= 60)
        {
            elapsedTime = 0;
            timeInfo.Minutes++;

            if(timeInfo.Minutes / 60 >= 1)
            {
                timeInfo.Minutes = 0;
                timeInfo.Hours++;
            }

            if(timeInfo.Hours / 24 >= 1)
            {
                timeInfo.Hours = 0;
                timeInfo.Day++;
                DayPassed?.Invoke(timeInfo.Day);
            }

            Tick?.Invoke();
            TickInfo?.Invoke(timeInfo);
        }

        elapsedTime += Time.deltaTime;
    }
}
