using System;
using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    // 이벤트 인수를 정의하는 클래스
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    // 틱이 발생할 때 호출될 이벤트
    public static event EventHandler<OnTickEventArgs> OnTick;

    // 틱 타이머의 최대 간격 (초 당 틱을 몇번 부를것인가?)
    private const float TICK_TIMER_MAX = 0.05f;

    // 현재 틱 및 타이머 상태를 저장하는 변수
    private int tick;
    private float tickTimer;
    private double lastTickTime;

    private void Awake()
    {
        // 초기화: 틱 및 마지막 티크 시간을 서버 시간으로 설정
        tick = 0;
        lastTickTime = PhotonNetwork.Time;
    }

    private void Update()
    {
        // 현재 서버 시간을 가져옴
        double currentTime = PhotonNetwork.Time;

        // 포톤네트워크 델타타임
        float elapsedTime = (float)(currentTime - lastTickTime);
        lastTickTime = currentTime;


        tickTimer += elapsedTime;
        // 틱 타이머가 최대 값을 초과했는지 확인
        if (tickTimer >= TICK_TIMER_MAX)
        {
            tickTimer -= TICK_TIMER_MAX;
            // 남은 시간 계산 후 업데이트
            tick++;
            if (tick >= 20000)
            {
                tick -= 20000;
            }
            // OnTick 이벤트 호출
            OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
        }
    }
}