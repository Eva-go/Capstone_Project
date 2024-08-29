using System;
using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    // �̺�Ʈ �μ��� �����ϴ� Ŭ����
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    // ƽ�� �߻��� �� ȣ��� �̺�Ʈ
    public static event EventHandler<OnTickEventArgs> OnTick;

    // ƽ Ÿ�̸��� �ִ� ���� (�� �� ƽ�� ��� �θ����ΰ�?)
    private const float TICK_TIMER_MAX = 0.05f;

    // ���� ƽ �� Ÿ�̸� ���¸� �����ϴ� ����
    private int tick;
    private float tickTimer;
    private double lastTickTime;

    private void Awake()
    {
        // �ʱ�ȭ: ƽ �� ������ Ƽũ �ð��� ���� �ð����� ����
        tick = 0;
        lastTickTime = PhotonNetwork.Time;
    }

    private void Update()
    {
        // ���� ���� �ð��� ������
        double currentTime = PhotonNetwork.Time;

        // �����Ʈ��ũ ��ŸŸ��
        float elapsedTime = (float)(currentTime - lastTickTime);
        lastTickTime = currentTime;


        tickTimer += elapsedTime;
        // ƽ Ÿ�̸Ӱ� �ִ� ���� �ʰ��ߴ��� Ȯ��
        if (tickTimer >= TICK_TIMER_MAX)
        {
            tickTimer -= TICK_TIMER_MAX;
            // ���� �ð� ��� �� ������Ʈ
            tick++;
            if (tick >= 20000)
            {
                tick -= 20000;
            }
            // OnTick �̺�Ʈ ȣ��
            OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
        }
    }
}