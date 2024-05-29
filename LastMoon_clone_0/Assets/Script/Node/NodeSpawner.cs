using UnityEngine;
using Photon.Pun; // Photon PUN ���ӽ����̽� �߰�
using System.Collections;

public class NodeSpawner : MonoBehaviourPun // Photon�� MonoBehaviourPun ���
{
    public GameObject[] itemPrefabs; // ������ ������ �迭
    public MeshCollider spawnArea; // ���� ������ mesh collider

    private float minSpawnInterval = 3f; // �ּ� ���� ����
    private float maxSpawnInterval = 10f; // �ִ� ���� ����
    private int totalItemsToSpawn = 1000; // �� ������ ������ ��
    private int itemsSpawned = 0; // ���� ������ ������ ��

    private void Start()
    {
        // ������ Ŭ���̾�Ʈ�� ���� �ڷ�ƾ ����
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnItems());
        }
    }

    private IEnumerator SpawnItems()
    {
        // �������� ��� ������ ������ �ݺ�
        while (itemsSpawned < totalItemsToSpawn)
        {
            // ������ �ð� ������ ����
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // ������ ��ġ�� ����
            Vector3 randomPoint = GetRandomPointOnColliderSurface(spawnArea);

            // ���� ���� �ִ��� Ȯ��
            RaycastHit hit;
            if (Physics.Raycast(randomPoint, Vector3.down, out hit, Mathf.Infinity))
            {
                randomPoint = hit.point; // ���� ��ġ�� ���� ���� ����
            }

            // ������ �������� �����Ͽ� ����
            int randomIndex = Random.Range(0, itemPrefabs.Length);

            // ������ ������ ��� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("SpawnItem", RpcTarget.AllBuffered, randomIndex, randomPoint);

            // ������ ������ �� ����
            itemsSpawned++;
        }
    }

    // mesh collider ������ ������ ����Ʈ�� ��ȯ�ϴ� �޼���
    private Vector3 GetRandomPointOnColliderSurface(MeshCollider collider)
    {
        Vector3 randomPoint = Vector3.zero;
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        // ������ ���� ����
        randomPoint.x = Random.Range(min.x, max.x);
        randomPoint.y = max.y; // collider�� ��ܿ� ��ġ
        randomPoint.z = Random.Range(min.z, max.z);

        return randomPoint;
    }

    // RPC �޼���: ��� Ŭ���̾�Ʈ���� �������� ����
    [PunRPC]
    private void SpawnItem(int prefabIndex, Vector3 position)
    {
        if (prefabIndex < 0 || prefabIndex >= itemPrefabs.Length)
        {
            Debug.LogError("Invalid prefab index");
            return;
        }

        Instantiate(itemPrefabs[prefabIndex], position, Quaternion.identity);
    }
}