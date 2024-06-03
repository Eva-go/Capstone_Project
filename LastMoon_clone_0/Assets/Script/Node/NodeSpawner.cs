using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NodeSpawner : MonoBehaviourPun
{
    // ������ ������ ������ �迭
    public GameObject[] itemPrefabs;

    // �޽� �ݶ��̴�
    public MeshCollider spawnArea;

    // ������ ������ ����
    public int itemCount = 10;

    // �� ������ ���� �ּ� �� �ִ� �Ÿ�
    public float minDistance = 2f;
    public float maxDistance = 5f;

    // �� ���� ������Ʈ�� Transform
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

            // PhotonNetwork.Instantiate�� ���� �����ϰ� �θ� ����
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
            max.y + 1f, // �޽� �ݶ��̴� ���� �ణ�� ������ �߰�
            Random.Range(min.z, max.z)
        );

        // �Ʒ��� ����ĳ��Ʈ�Ͽ� �޽� �ݶ��̴��� �ֻ�� ǥ���� ã��
        RaycastHit hit;
        if (Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            randomPosition.y = hit.point.y + 0.1f; // �������� �޽� �ݶ��̴� ���� ��ġ�ϵ��� �ణ ���� �̵�
        }

        return randomPosition;
    }
}