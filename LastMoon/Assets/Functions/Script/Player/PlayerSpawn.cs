using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public static List<Transform> points = new List<Transform>(); // Transform 리스트로 수정
    private GameObject player;

    void Start()
    {
        // "Building" 태그가 지정된 모든 건물 오브젝트 찾기
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("APT");

        // 디버그 로그: 찾은 건물 오브젝트 수 출력
        Debug.Log("Number of buildings found: " + buildings.Length);

        foreach (GameObject building in buildings)
        {
            // 디버그 로그: 각 건물 오브젝트 이름 출력
            Debug.Log("Building name: " + building.name);

            // 건물 하위 오브젝트 중 이름이 "point"인 오브젝트 찾기
            Transform[] childTransforms = building.GetComponentsInChildren<Transform>();
            foreach (Transform child in childTransforms)
            {
                if (child.name.Equals("Point"))
                {
                    points.Add(child);
                    // 디버그 로그: 찾은 point 오브젝트 이름과 위치 출력
                    Debug.Log("Found point: " + child.name + " at position " + child.position);
                }
            }
        }

        if (points.Count > 0)
        {
            int idx = Random.Range(1, points.Count-1); // 랜덤 스폰 포인트 선택
            SpawnPlayer(idx);
        }
        else
        {
            Debug.LogError("스폰 포인트가 없거나 충분하지 않습니다.");
        }
    }

    public void SpawnPlayer(int idx)
    {
        Debug.Log("Spawning player at point: " + idx);
        Debug.Log("Point position: " + points[idx].position);
        Debug.Log("Point rotation: " + points[idx].rotation);

        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation); // 플레이어 인스턴스화
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
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // 플레이어 소유권 설정
            Debug.Log("Player spawned and ownership transferred.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}