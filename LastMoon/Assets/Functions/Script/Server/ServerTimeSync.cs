using UnityEngine;
using Photon.Pun;

public class ServerTimeSync : MonoBehaviourPun
{
    public float syncInterval = 1.0f; // 서버 시간 동기화 주기 (초)
    private float lastSyncTime;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            lastSyncTime = Time.time;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time - lastSyncTime >= syncInterval)
            {
                lastSyncTime = Time.time;
                // 서버 시간을 클라이언트에 동기화
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("UpdateServerTime", RpcTarget.All, Time.time);
            }
        }
    }

    [PunRPC]
    void UpdateServerTime(float serverTime)
    {
        ClientTimeSync.Instance.UpdateServerTime(serverTime);
    }
}