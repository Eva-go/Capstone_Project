using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class APTSpawner : MonoBehaviourPunCallbacks
{
    // ������ ������ ������ �迭
    public GameObject[] itemPrefabs;

    // �޽� �ݶ��̴�
    public MeshCollider spawnArea;
    // ������ ������ ����
    public int itemCount = 10;

    // �� ������ ���� �ּ� �� �ִ� �Ÿ�
    public float minDistance;
    public float maxDistance;
    public float dir;

    // �� ���� ������Ʈ�� Transform
    public Transform parentTransform;

    void Start()
    {
        SpawnItems(); // ��� Ŭ���̾�Ʈ���� ������ ����
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
                spawnPosition = GetRandomPositionInCircle(spawnArea);

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

    Vector3 GetRandomPositionInCircle(MeshCollider meshCollider)
    {
        Vector3 center = meshCollider.bounds.center;
        float radius = Mathf.Min(meshCollider.bounds.extents.x, meshCollider.bounds.extents.z); // �޽� �ݶ��̴��� ����, ���� �� ���� ���� ���������� ����

        float angle = Random.Range(0f, Mathf.PI * dir); // 0���� 2�� ������ ������ �������� ����
        float distance = Random.Range(0f, radius); // �߽ɿ��� ���������� ������ �Ÿ� ����

        Vector3 randomPosition = new Vector3(
            center.x + Mathf.Cos(angle) * distance,
            center.y,
            center.z + Mathf.Sin(angle) * distance
        );

        // �Ʒ��� ����ĳ��Ʈ�Ͽ� �޽� �ݶ��̴��� ǥ���� ã��
        RaycastHit hit;
        if (Physics.Raycast(randomPosition + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            randomPosition.y = hit.point.y; // �޽� �ݶ��̴��� ǥ�鿡 ��Ȯ�� ��ġ�ϵ��� y�� ����
        }

        return randomPosition;
    }
}