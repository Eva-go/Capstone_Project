using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    public float tickInterval = 10f; // ƽ ���� (�� ����)
    private double nextTickTime;

    void Start()
    {
        // PhotonNetwork.Time�� double �����̹Ƿ� ������ double�� �����մϴ�.
        nextTickTime = PhotonNetwork.Time + tickInterval;
    }

    void Update()
    {
        // ���� ��Ʈ��ũ �ð��� double �������� �����ɴϴ�.
        if (PhotonNetwork.Time >= nextTickTime)
        {
            ExecuteTick();
            nextTickTime += tickInterval; // ���� ƽ �ð��� double �������� �����մϴ�.
        }
    }

    void ExecuteTick()
    {
        // �̰��� ƽ���� ������ �ڵ带 �ֽ��ϴ�.
        Debug.Log("Tick executed at: " + PhotonNetwork.Time);
    }
}