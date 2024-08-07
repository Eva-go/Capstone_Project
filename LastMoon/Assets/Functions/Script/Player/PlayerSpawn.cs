using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static List<Transform> points = new List<Transform>(); // Transform 리스트로 수정
    private static GameObject player;

    public static void OnBuildingCreated()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("APT");

        foreach (GameObject building in buildings)
        {
            Transform[] childTransforms = building.GetComponentsInChildren<Transform>();
            foreach (Transform child in childTransforms)
            {
                if (child.name.Equals("Point"))
                {
                    points.Add(child);
                }
            }

        }

        if (points.Count > 0)
        {
            int idx = Random.Range(0, points.Count);
            SpawnPlayer(idx);
        }
        else
        {
            Debug.LogError("스폰 포인트가 없거나 충분하지 않습니다.");
        }
    }


    public static void SpawnPlayer(int idx)
    {
        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
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
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            Debug.Log("Player spawned and ownership transferred.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}