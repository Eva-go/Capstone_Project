using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public RectTransform timerImageRectTransform; // Ÿ�̸Ӹ� ǥ���� RectTransform
    private double startTime; // ���� ���� �ð�
    private int maxTime; // �ִ� �ð� (�� ����)
    private float startX = 234f; // �̹����� ���� X ��ǥ
    private float endX = -234f; // �̹����� �� X ��ǥ

    void Start()
    {
        // ���� ���� �ð��� �����Ǿ� �ִ��� Ȯ��
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("startTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["startTime"];
        }
        else
        {
            startTime = PhotonNetwork.Time; // ���� �ð��� �������� ���� ��� ���� �ð����� ����
        }

        // �ִ� �ð��� �����Ǿ� �ִ��� Ȯ��
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("maxTime"))
        {
            maxTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxTime"] * 60; // �� ������ �� ������ ��ȯ
        }
    }

    void Update()
    {
        // ��� �ð� ���
        double elapsedTime = PhotonNetwork.Time - startTime;
        // ���� �ð� ���
        double remainingTime = maxTime - elapsedTime;

        // ���� �ð��� 0 ������ ��� ó��
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            // ���� ���� ���� �߰� ����
        }

        // ���� �ð� ���� ���
        float remainingTimeRatio = (float)(remainingTime / maxTime);
        // �̹����� ���ο� X ��ǥ ���
        float newX = Mathf.Lerp(endX, startX, remainingTimeRatio);

        // �̹����� X ��ǥ ������Ʈ
        Vector3 newPosition = timerImageRectTransform.localPosition;
        newPosition.x = newX;
        timerImageRectTransform.localPosition = newPosition;

        // F1 Ű�� ������ �ð��� 5�о� ����
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("ReduceTime", RpcTarget.AllBuffered, 5);
            Debug.Log("f1");
        }
    }

    [PunRPC]
    void ReduceTime(int minutes)
    {
        maxTime -= minutes * 60;
    }
}