using UnityEngine;

public class TestPooler : MonoBehaviour
{
    [SerializeField] Rigidbody rbody;
    [SerializeField] Renderer render;

    [SerializeField] bool shouldReturnToPool = true; // Default to true
    [SerializeField] Vector3 force = new Vector3(1f, 0f, 0f);
    [SerializeField] string targetObjectName = "Poi_EndPoint"; // ���� ������Ʈ �̸�
    [SerializeField] Vector3 pos = new Vector3();
    private void Start()
    {
        pos = transform.parent.position;
        pos.y = transform.parent.position.y + 1;
        gameObject.transform.position = pos;
        // ������Ʈ�� Ư�� ��ġ�� ����
        SpawnObjectAtLocation(gameObject.name, gameObject.transform.position);
    }

    void SpawnObjectAtLocation(string tag, Vector3 position)
    {
        // Ǯ���� ������Ʈ�� �����ϰ� ��ġ�� ����
        GameObject spawnedObject = ObjectPooler.SpawnFromPool(tag, position);
        // �߰����� ������ �ʿ��ϴٸ� ���⼭ ó��
    }

    private void OnEnable()
    {
        rbody.velocity = force;

        if (shouldReturnToPool)
        {
            Invoke(nameof(DectiveDelay), 5);
        }
    }

    public void Setup(bool returnToPool)
    {
        shouldReturnToPool = returnToPool;
    }

    void DectiveDelay()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (shouldReturnToPool)
        {
            ObjectPooler.ReturnToPool(gameObject);
        }
        CancelInvoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �̸��� targetObjectName�� ��ġ�ϴ��� Ȯ��
        if (collision.gameObject.name == targetObjectName)
        {
            // ������Ʈ ��Ȱ��ȭ �� Ǯ�� ��ȯ
            gameObject.SetActive(false);
            ObjectPooler.ReturnToPool(gameObject);
        }
    }
}