using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    public static TickTimer Instance { get; private set; }

    private List<Timer> timers = new List<Timer>();
    private float serverTimeOffset; // 서버와 클라이언트 간의 시간 오프셋

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 서버 시간 오프셋을 계산합니다. 이 값은 처음 시작할 때 서버와 클라이언트의 시간 차이를 맞추기 위한 값입니다.
        serverTimeOffset = PhotonNetwork.ServerTimestamp / 1000f - Time.time;
    }

    public void StartTickTimer(float interval, System.Action callback)
    {
        Timer timer = new Timer
        {
            Interval = interval,
            Callback = callback,
            NextTickTime = GetCurrentServerTime() + interval
        };
        timers.Add(timer);
    }

    public void StopTickTimer(System.Action callback)
    {
        Timer timerToRemove = timers.FirstOrDefault(t => t.Callback == callback);
        if (timerToRemove != null)
        {
            timers.Remove(timerToRemove);
        }
    }

    public bool IsRunning(System.Action callback)
    {
        return timers.Any(t => t.Callback == callback);
    }

    private void Update()
    {
        float currentServerTime = GetCurrentServerTime();

        foreach (var timer in timers.ToArray())
        {
            if (currentServerTime >= timer.NextTickTime)
            {
                timer.Callback.Invoke();
                timer.NextTickTime = currentServerTime + timer.Interval;
            }
        }
    }

    private float GetCurrentServerTime()
    {
        return PhotonNetwork.ServerTimestamp / 1000f - serverTimeOffset;
    }

    private class Timer
    {
        public float Interval { get; set; }
        public float NextTickTime { get; set; }
        public System.Action Callback { get; set; }
    }
}