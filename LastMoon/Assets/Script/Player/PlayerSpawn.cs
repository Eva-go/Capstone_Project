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

        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(); // ���� ����Ʈ ��������
        if (points.Length > 1)
        {
            idx = Random.Range(1, points.Length); // ���� ���� ����Ʈ ����
            GameObject player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // �÷��̾� �ν��Ͻ�ȭ
            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // �÷��̾� ������ ����
            player.name = GameValue.nickNumae;
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }
    }

}