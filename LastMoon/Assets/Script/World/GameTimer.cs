using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public Image timerImage; // UI Image (Fill type) to show the timer
    private double startTime;
    private int maxTime;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("startTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["startTime"];
        }
        else
        {
            startTime = PhotonNetwork.Time; // Fallback in case startTime is not set
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("maxTime"))
        {
            maxTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxTime"] * 60; // Convert minutes to seconds
        }
    }

    void Update()
    {
        double elapsedTime = PhotonNetwork.Time - startTime;
        double remainingTime = maxTime - elapsedTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            // 게임 종료 로직 추가 가능
        }

        timerImage.fillAmount = (float)(remainingTime / maxTime);
    }
}