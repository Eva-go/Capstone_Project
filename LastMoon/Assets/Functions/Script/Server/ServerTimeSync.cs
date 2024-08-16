using UnityEngine;
using Photon.Pun;

public class ServerTimeSync : MonoBehaviourPun
{
    public float syncInterval = 1.0f; // ���� �ð� ����ȭ �ֱ� (��)
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
                // ���� �ð��� Ŭ���̾�Ʈ�� ����ȭ
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