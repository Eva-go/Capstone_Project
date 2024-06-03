using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NodeSpawner : MonoBehaviourPun
{
    // 스폰할 아이템 프리팹 배열
    public GameObject[] itemPrefabs;

    // 메쉬 콜라이더
    public MeshCollider spawnArea;

    // 스폰할 아이템 갯수
    public int itemCount = 10;

    // 각 아이템 간의 최소 및 최대 거리
    public float minDistance = 2f;
    public float maxDistance = 5f;

    // 빈 게임 오브젝트의 Transform
    public Transform parentTransform;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnItems();
        }
    }

    void SpawnItems()
    {
        List<Vector3> spawnedPositions = new List<Vector3>();

        for (int i = 0; i < itemCount; i++)
        {
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            Vector3 spawnPosition;
            bool positionValid;

            do
            {
                positionValid = true;
                spawnPosition = GetRandomPositionAboveMeshCollider(spawnArea);

                foreach (Vector3 pos in spawnedPositions)
                {
                    if (Vector3.Distance(spawnPosition, pos) < minDistance)
                    {
                        positionValid = false;
                        break;
                    }
                }
            }
            while (!positionValid);

            spawnedPositions.Add(spawnPosition);

            // PhotonNetwork.Instantiate를 통해 생성하고 부모 설정
            GameObject spawnedItem = PhotonNetwork.Instantiate(itemPrefab.name, spawnPosition, Quaternion.identity);
            spawnedItem.transform.SetParent(parentTransform);
        }
    }

    Vector3 GetRandomPositionAboveMeshCollider(MeshCollider meshCollider)
    {
        Vector3 min = meshCollider.bounds.min;
        Vector3 max = meshCollider.bounds.max;

        Vector3 randomPosition = new Vector3(
            Random.Range(min.x, max.x),
            max.y + 1f, // 메쉬 콜라이더 위로 약간의 오프셋 추가
            Random.Range(min.z, max.z)
        );

        // 아래로 레이캐스트하여 메쉬 콜라이더의 최상단 표면을 찾음
        RaycastHit hit;
        if (Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            randomPosition.y = hit.point.y + 0.1f; // 아이템이 메쉬 콜라이더 위에 위치하도록 약간 위로 이동
        }

        return randomPosition;
    }
}