using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    void Start()
    {

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(); // ���� ����Ʈ ��������
        if (points.Length > 1)
        {
            int idx = Random.Range(1, points.Length); // ���� ���� ����Ʈ ����
            GameObject player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // �÷��̾� �ν��Ͻ�ȭ
            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // �÷��̾� ������ ����
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }
    }

}