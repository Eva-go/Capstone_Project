using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [Header("Object Pool Settings")]
    [SerializeField] private string objectTag; // 풀에서 꺼낼 오브젝트의 태그
    [SerializeField] private float spawnDistance = 1f; // 오브젝트를 스폰할 거리

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // 스폰 위치 오프셋
    [SerializeField] private Vector3 spawnRotation = Vector3.zero; // 스폰 회전 값 (Euler Angles)

    private void Update()
    {
        // 키 입력으로 오브젝트 스폰
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if (string.IsNullOrEmpty(objectTag))
        {
            Debug.LogError("ObjectTag is not assigned.");
            return;
        }

        // 오브젝트를 스폰할 위치를 StartPoint의 위치와 오프셋으로 설정
        Vector3 spawnPosition = transform.position + transform.TransformDirection(spawnOffset);
        Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation);

        // 오브젝트 풀에서 오브젝트를 꺼냄
        GameObject obj = ObjectPooler.SpawnFromPool(objectTag, spawnPosition, spawnQuaternion);

        if (obj == null)
        {
            Debug.LogWarning("Failed to spawn object from pool.");
        }
    }
}