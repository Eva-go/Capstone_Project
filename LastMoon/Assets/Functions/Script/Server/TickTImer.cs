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

    // 티크가 발생할 때 호출될 이벤트
    public static event EventHandler<OnTickEventArgs> OnTick;

    // 티크 타이머의 최대 간격 (초 단위)
    private const float TICK_TIMER_MAX = 20f;

    // 현재 티크 및 타이머 상태를 저장하는 변수
    private int tick;
    private double lastTickTime;

    private void Awake()
    {
        // 초기화: 티크 및 마지막 티크 시간을 서버 시간으로 설정
        tick = 0;
        lastTickTime = PhotonNetwork.Time;
    }

    private void Update()
    {
        // 현재 서버 시간을 가져옴
        double currentTime = PhotonNetwork.Time;

        // 마지막 티크 시간과 현재 시간의 차이를 계산
        float elapsedTime = (float)(currentTime - lastTickTime);

        // 티크 타이머가 최대 값을 초과했는지 확인
        if (elapsedTime >= TICK_TIMER_MAX)
        {
            // 남은 시간 계산 후 업데이트
            lastTickTime = currentTime - (elapsedTime % TICK_TIMER_MAX);
            tick++;

            // OnTick 이벤트 호출
            OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
        }
    }
}