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

    // Ƽũ�� �߻��� �� ȣ��� �̺�Ʈ
    public static event EventHandler<OnTickEventArgs> OnTick;

    // Ƽũ Ÿ�̸��� �ִ� ���� (�� ����)
    private const float TICK_TIMER_MAX = 20f;

    // ���� Ƽũ �� Ÿ�̸� ���¸� �����ϴ� ����
    private int tick;
    private double lastTickTime;

    private void Awake()
    {
        // �ʱ�ȭ: Ƽũ �� ������ Ƽũ �ð��� ���� �ð����� ����
        tick = 0;
        lastTickTime = PhotonNetwork.Time;
    }

    private void Update()
    {
        // ���� ���� �ð��� ������
        double currentTime = PhotonNetwork.Time;

        // ������ Ƽũ �ð��� ���� �ð��� ���̸� ���
        float elapsedTime = (float)(currentTime - lastTickTime);

        // Ƽũ Ÿ�̸Ӱ� �ִ� ���� �ʰ��ߴ��� Ȯ��
        if (elapsedTime >= TICK_TIMER_MAX)
        {
            // ���� �ð� ��� �� ������Ʈ
            lastTickTime = currentTime - (elapsedTime % TICK_TIMER_MAX);
            tick++;

            // OnTick �̺�Ʈ ȣ��
            OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
        }
    }
}