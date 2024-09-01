using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    //public static List<Transform> points = new List<Transform>(); // Transform ����Ʈ�� ����
    private static GameObject player;

    private void Start()
    {
        OnBuildingCreated();
    }
    public  void OnBuildingCreated()
    {
        Transform parentTransform = GameObject.Find("SpawnPoint").transform;
        List<Transform> directChildren = new List<Transform>();

        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            directChildren.Add(child);
        }

        if (directChildren.Count > 0)
        {
            int idx = Random.Range(0, directChildren.Count);
            Debug.Log("��������Ʈ: " + idx);
            SpawnPlayer(idx, directChildren.ToArray());
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� �����ϴ�.");
        }
    }


    public void SpawnPlayer(int idx,Transform[] points)
    {
        player = PhotonNetwork.Instantiate("Player", points[NetworkManager.PlayerID].position, points[idx].rotation);
        if (player != null)
        {
            GameObject playerprefab = Instantiate(player, transform.position, transform.rotation);
            PlayerController.Instance.SetPlayer(playerprefab); // ������ �÷��̾ PlayerManager�� ���
            player.name = PhotonNetwork.LocalPlayer.NickName;
            Transform OtherPlayer = player.transform.Find("OtherPlayer");
            Transform LocalPlayer = player.transform.Find("LocalPlayer");
            Transform Tool = player.transform.Find("Player001");
            Transform T_LocalPlayerTool = player.transform.Find("ToolCamera");

            if (OtherPlayer != null) OtherPlayer.gameObject.SetActive(false);
            if (LocalPlayer != null) LocalPlayer.gameObject.SetActive(true);
            if (Tool != null) Tool.gameObject.SetActive(false);
            if (T_LocalPlayerTool != null) T_LocalPlayerTool.gameObject.SetActive(true);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            Debug.Log("Player spawned and ownership transferred.");
        }
    }
}