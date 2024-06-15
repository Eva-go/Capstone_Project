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
    public GameObject LocalPlayer;
    
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
            Transform LocalTool = player.transform.Find("Player001");
            OtherPlayer.gameObject.SetActive(false);
            LocalPlayer.gameObject.SetActive(true);
            LocalTool.gameObject.SetActive(false);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // �÷��̾� ������ ����
           
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }

    } 
}