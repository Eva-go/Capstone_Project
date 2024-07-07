using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static Transform[] points;
    public static int idx;
    private GameObject player;

    void Start()
    {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>(); // ���� ����Ʈ ��������
        if (points.Length > 1)
        {
            idx = Random.Range(1, points.Length); // ���� ���� ����Ʈ ����
            SpawnPlayer();
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player at point: " + idx);
        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // �÷��̾� �ν��Ͻ�ȭ
        if (player != null)
        {
            player.name = PhotonNetwork.LocalPlayer.NickName;
            Transform OtherPlayer = player.transform.Find("OtherPlayer");
            Transform LocalPlayer = player.transform.Find("LocalPlayer");
            Transform Tool = player.transform.Find("Player001");
            Transform T_LocalPlayerTool = player.transform.Find("ToolCamera");
            OtherPlayer.gameObject.SetActive(false);
            LocalPlayer.gameObject.SetActive(true);
            Tool.gameObject.SetActive(false);
            T_LocalPlayerTool.gameObject.SetActive(true);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // �÷��̾� ������ ����
            Debug.Log("Player spawned and ownership transferred.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}