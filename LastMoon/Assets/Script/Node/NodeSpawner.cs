using UnityEngine;
using Photon.Pun; // Photon PUN 네임스페이스 추가
using System.Collections;

public class NodeSpawner : MonoBehaviourPun // Photon의 MonoBehaviourPun 사용
{
    public GameObject[] itemPrefabs; // 아이템 프리팹 배열
    public MeshCollider spawnArea; // 스폰 영역의 mesh collider

    private float minSpawnInterval = 3f; // 최소 스폰 간격
    private float maxSpawnInterval = 10f; // 최대 스폰 간격
    private int totalItemsToSpawn = 1000; // 총 스폰할 아이템 수
    private int itemsSpawned = 0; // 현재 스폰된 아이템 수

    private void Start()
    {
        // 마스터 클라이언트만 스폰 코루틴 시작
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnItems());
        }
    }

    private IEnumerator SpawnItems()
    {
        // 아이템을 모두 스폰할 때까지 반복
        while (itemsSpawned < totalItemsToSpawn)
        {
            // 랜덤한 시간 간격을 생성
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // 랜덤한 위치를 생성
            Vector3 randomPoint = GetRandomPointOnColliderSurface(spawnArea);

            // 지면 위에 있는지 확인
            RaycastHit hit;
            if (Physics.Raycast(randomPoint, Vector3.down, out hit, Mathf.Infinity))
            {
                randomPoint = hit.point; // 스폰 위치를 지면 위로 조정
            }

            // 랜덤한 아이템을 선택하여 스폰
            int randomIndex = Random.Range(0, itemPrefabs.Length);

            // 아이템 스폰을 모든 클라이언트와 동기화
            photonView.RPC("SpawnItem", RpcTarget.AllBuffered, randomIndex, randomPoint);

            // 스폰된 아이템 수 증가
            itemsSpawned++;
        }
    }

    // mesh collider 내에서 랜덤한 포인트를 반환하는 메서드
    private Vector3 GetRandomPointOnColliderSurface(MeshCollider collider)
    {
        Vector3 randomPoint = Vector3.zero;
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        // 랜덤한 점을 생성
        randomPoint.x = Random.Range(min.x, max.x);
        randomPoint.y = max.y; // collider의 상단에 위치
        randomPoint.z = Random.Range(min.z, max.z);

        return randomPoint;
    }

    // RPC 메서드: 모든 클라이언트에서 아이템을 스폰
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