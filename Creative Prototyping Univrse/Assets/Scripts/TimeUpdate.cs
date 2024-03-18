using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUpdate : MonoBehaviour
{
    private void Update() { Timers.Instance.UpdateTimers(); }
}

public class Timer
{
    public float time;
    public bool countingTime;

    public Timer(bool paused)
    {
        time = 0f; countingTime = !paused;
    }
}

public sealed class Timers
{
    //Singleton
    private Timers()
    {
        this.timers = new List<Timer>();
    }

    private static Timers instance;
    public static Timers Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Timers();
            }
            return instance;
        }
    }

    private List<Timer> timers;

    //Method to update the timers
    public void UpdateTimers()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i].countingTime && !GetIfFinishedTimer(i)) { timers[i].time += Time.deltaTime; }
        }
    }

    //<----------------------METHODS THAT USES THE TIME CONTROLLER-------------------------->

    //Method to change the time_count
    private void Change_Time_Count(int idx, bool status)
    {
        timers[idx].countingTime = status;
    }

    //<------------------------METHODS TO USE THE TIME CONTROLLER--------------------------->

    //method to create a timer
    public int CreateTimer(bool paused) { 
        timers.Add(new Timer(paused));
        return timers.Count - 1;
    }

    //method to get the time value of a timer
    public float GetTime(int idx) { return timers[idx].time; }

    //method to set a time value to a timer
    public void SetTime(int idx, float time) { timers[idx].time = time; }

    //method to know if the timer has arrived to a certain time
    public bool IsTime(int idx, float seconds) { return timers[idx].time > seconds; }

    public void ResumeTimer(int idx) { Change_Time_Count(idx, true); } //method to pause a timer
    public void PauseTimer(int idx) { Change_Time_Count(idx, false); } //method to pause a timer

    //method to reanude all timers
    public void ResumeAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { ResumeTimer(i); }
    }

    //method to pause all timers
    public void PauseAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { PauseTimer(i); }
    }

    public void RestartTimer(int idx) { timers[idx].time = 0.0f; } //method to restart a timer

    //method to restart all timers
    public void RestartAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { RestartTimer(i); }
    }

    public void ResetTimer(int idx)
    {
        RestartTimer(idx);
        PauseTimer(idx);
    }

    public bool WaitTime(int idx, float time)
    {
        ResumeTimer(idx);

        return IsTime(idx, time);
    }

    public bool WaitTimeWithReset(int idx, float time)
    {
        ResumeTimer(idx);

        bool condition = IsTime(idx, time);

        if (condition) { ResetTimer(idx); }

        return condition;
    }

    //method to delete all timers
    public void DeleteTimers() { timers.Clear(); }

    //method to get if a timer is "finished"
    public bool GetIfFinishedTimer(int idx)
    {
        return GetTime(idx) < 0.0f;
    }

    //method to set a timer as "finished" with a negative time
    public void SetFinishedTimer(int idx)
    {
        SetTime(idx, -1.0f);
    }
}
