using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static Transform[] points;
    public static int idx;
    private GameObject player;
    
    void Start()
    {

        points = GameObject.Find("APTSpawner").GetComponentsInChildren<Transform>(); // ���� ����Ʈ ��������
        if (points.Length > 1)
        {
            idx = Random.Range(1, points.Length); // ���� ���� ����Ʈ ����


            player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // �÷��̾� �ν��Ͻ�ȭ
            player.name = GameValue.nickNumae;
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
           
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }

    } 
}