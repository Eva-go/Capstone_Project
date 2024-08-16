using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [Header("Object Pool Settings")]
    [SerializeField] private string objectTag; // Ǯ���� ���� ������Ʈ�� �±�
    [SerializeField] private float spawnDistance = 1f; // ������Ʈ�� ������ �Ÿ�

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // ���� ��ġ ������
    [SerializeField] private Vector3 spawnRotation = Vector3.zero; // ���� ȸ�� �� (Euler Angles)

    private void Update()
    {
        // Ű �Է����� ������Ʈ ����
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

        // ������Ʈ�� ������ ��ġ�� StartPoint�� ��ġ�� ���������� ����
        Vector3 spawnPosition = transform.position + transform.TransformDirection(spawnOffset);
        Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation);

        // ������Ʈ Ǯ���� ������Ʈ�� ����
        GameObject obj = ObjectPooler.SpawnFromPool(objectTag, spawnPosition, spawnQuaternion);

        if (obj == null)
        {
            Debug.LogWarning("Failed to spawn object from pool.");
        }
    }
}