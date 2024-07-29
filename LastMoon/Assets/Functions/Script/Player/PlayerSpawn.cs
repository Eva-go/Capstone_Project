using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static List<Transform> points = new List<Transform>(); // Transform ����Ʈ�� ����
    private GameObject player;

    void Start()
    {
        // "Building" �±װ� ������ ��� �ǹ� ������Ʈ ã��
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("APT");

        // ����� �α�: ã�� �ǹ� ������Ʈ �� ���
        Debug.Log("Number of buildings found: " + buildings.Length);

        foreach (GameObject building in buildings)
        {
            // ����� �α�: �� �ǹ� ������Ʈ �̸� ���
            Debug.Log("Building name: " + building.name);

            // �ǹ� ���� ������Ʈ �� �̸��� "point"�� ������Ʈ ã��
            Transform[] childTransforms = building.GetComponentsInChildren<Transform>();
            foreach (Transform child in childTransforms)
            {
                if (child.name.Equals("Point"))
                {
                    points.Add(child);
                    // ����� �α�: ã�� point ������Ʈ �̸��� ��ġ ���
                    Debug.Log("Found point: " + child.name + " at position " + child.position);
                }
            }
        }

        if (points.Count > 0)
        {
            int idx = Random.Range(1, points.Count-1); // ���� ���� ����Ʈ ����
            SpawnPlayer(idx);
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }
    }

    public void SpawnPlayer(int idx)
    {
        Debug.Log("Spawning player at point: " + idx);
        Debug.Log("Point position: " + points[idx].position);
        Debug.Log("Point rotation: " + points[idx].rotation);

        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation); // �÷��̾� �ν��Ͻ�ȭ
        if (player != null)
        {
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
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // �÷��̾� ������ ����
            Debug.Log("Player spawned and ownership transferred.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}