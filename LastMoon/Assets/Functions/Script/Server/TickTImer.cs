using System;
using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;

    private static bool isRunning = false;
    private static float tickInterval = 0.05f;
    private static double lastTickTime;
    private static Action tickAction;

    private void Update()
    {
        if (isRunning)
        {
            UpdateTick();
        }
    }

    public static void StartTickTimer(float interval, Action action)
    {
        if (!isRunning)
        {
            tickInterval = interval;
            lastTickTime = PhotonNetwork.Time;
            isRunning = true;
            tickAction = action;
        }
    }

    public static void StopTickTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            tickAction = null;
        }
    }

    private void UpdateTick()
    {
        double currentTime = PhotonNetwork.Time;
        float elapsedTime = (float)(currentTime - lastTickTime);

        if (elapsedTime >= tickInterval)
        {
            lastTickTime = currentTime - (elapsedTime % tickInterval);
            OnTick?.Invoke(null, new OnTickEventArgs { tick = (int)(currentTime / tickInterval) });

            tickAction?.Invoke(); // Invoke the action on each tick
        }
    }
}