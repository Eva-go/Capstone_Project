using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerScore : MonoBehaviourPunCallbacks
{
    public string playerName;
    public int score = 0;

    private void Start()
    {
        playerName = PhotonNetwork.LocalPlayer.NickName;
        score = GameValue.Money_total;

        // 로컬 플레이어의 커스텀 속성 설정
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties.Add("score", score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
}