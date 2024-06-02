using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static Transform[] points;
    public static int idx;
    void Start()
    {

        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(); // 스폰 포인트 가져오기
        if (points.Length > 1)
        {
            idx = Random.Range(1, points.Length); // 랜덤 스폰 포인트 선택
            GameObject player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // 플레이어 인스턴스화
            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // 플레이어 소유권 설정
            player.name = GameValue.nickNumae;
        }
        else
        {
            Debug.LogError("스폰 포인트가 없거나 충분하지 않습니다.");
        }
    }

}