using UnityEngine;

public class TestPooler : MonoBehaviour
{
    [SerializeField] Rigidbody rbody;
    [SerializeField] Renderer render;

    [SerializeField] bool shouldReturnToPool = true; // Default to true
    [SerializeField] Vector3 force = new Vector3(1f, 0f, 0f);
    [SerializeField] string targetObjectName = "Poi_EndPoint"; // 비교할 오브젝트 이름
    [SerializeField] Vector3 pos = new Vector3();
    private void Start()
    {
        pos = transform.parent.position;
        pos.y = transform.parent.position.y + 1;
        gameObject.transform.position = pos;
        // 오브젝트를 특정 위치에 스폰
        SpawnObjectAtLocation(gameObject.name, gameObject.transform.position);
    }

    void SpawnObjectAtLocation(string tag, Vector3 position)
    {
        // 풀에서 오브젝트를 스폰하고 위치를 설정
        GameObject spawnedObject = ObjectPooler.SpawnFromPool(tag, position);
        // 추가적인 설정이 필요하다면 여기서 처리
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
        // 충돌한 오브젝트의 이름이 targetObjectName과 일치하는지 확인
        if (collision.gameObject.name == targetObjectName)
        {
            // 오브젝트 비활성화 및 풀에 반환
            gameObject.SetActive(false);
            ObjectPooler.ReturnToPool(gameObject);
        }
    }
}