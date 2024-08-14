using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Timer
{
    protected float initialTime;
    protected float Time { get; set; }

    public bool IsRunning { get; protected set; }
    public float progress => Time/initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer (float value)
    {
        this.initialTime = value;
        IsRunning = false;
    }

    public void StartTimer()
    {
        Time = initialTime;
        if(!IsRunning )
        {
            IsRunning = true;
            OnTimerStart?.Invoke();
        }       
    }
    public void StopTimer()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop?.Invoke();
        }
    }
    public bool Pause => !IsRunning;
    public bool Resume => IsRunning;
    public abstract void Tick(float deltaTime);
}
public class CountDownTimer : Timer
{
    public CountDownTimer(float value) : base(value)
    {
    }
    public override void Tick(float deltaTime)
    {
        if(IsRunning && Time > 0)
        {
            Time -= deltaTime;
        }
        if (IsRunning && Time <= 0)
        {
            StopTimer();
        }
    }
    public bool IsFinish => Time <= 0;
    public void Reset() => Time = initialTime;
    public void Reset(float value)
    {
        initialTime = value;
        Reset();
    }
}
public class StopWatchTimer : Timer
{
    public StopWatchTimer(float value) : base(0)
    {
    }

    public override void Tick(float deltaTime)
    {
        if(IsRunning)
        {
            Time += deltaTime;
        }
    }
    public void Reset() => Time = 0;
    public float GetTime() => Time;
}
