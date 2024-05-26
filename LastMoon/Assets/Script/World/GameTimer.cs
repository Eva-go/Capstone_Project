using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public RectTransform timerImageRectTransform; // 타이머를 표시할 RectTransform
    private double startTime; // 게임 시작 시간
    private int maxTime; // 최대 시간 (초 단위)
    private float startX = 234f; // 이미지의 시작 X 좌표
    private float endX = -234f; // 이미지의 끝 X 좌표

    void Start()
    {
        // 게임 시작 시간이 설정되어 있는지 확인
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("startTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["startTime"];
        }
        else
        {
            startTime = PhotonNetwork.Time; // 시작 시간이 설정되지 않은 경우 현재 시간으로 설정
        }

        // 최대 시간이 설정되어 있는지 확인
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("maxTime"))
        {
            maxTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxTime"] * 60; // 분 단위를 초 단위로 변환
        }
    }

    void Update()
    {
        // 경과 시간 계산
        double elapsedTime = PhotonNetwork.Time - startTime;
        // 남은 시간 계산
        double remainingTime = maxTime - elapsedTime;

        // 남은 시간이 0 이하일 경우 처리
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            // 게임 종료 로직 추가 가능
        }

        // 남은 시간 비율 계산
        float remainingTimeRatio = (float)(remainingTime / maxTime);
        // 이미지의 새로운 X 좌표 계산
        float newX = Mathf.Lerp(endX, startX, remainingTimeRatio);

        // 이미지의 X 좌표 업데이트
        Vector3 newPosition = timerImageRectTransform.localPosition;
        newPosition.x = newX;
        timerImageRectTransform.localPosition = newPosition;

        // F1 키를 누르면 시간을 5분씩 줄임
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