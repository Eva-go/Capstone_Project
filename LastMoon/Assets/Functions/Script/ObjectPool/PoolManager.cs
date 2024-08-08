using UnityEngine;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public ObjectPool objectPool;
    public Vector3 moveDirection = Vector3.right; // 이동 방향
    public float moveDistance = 10f; // 이동 거리

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 스페이스바를 누르면 풀에서 오브젝트를 가져옴
            GameObject obj = objectPool.GetObject();
            obj.transform.position = Vector3.zero; // 시작 위치 설정
            StartCoroutine(MoveObject(obj));
        }
    }

    private IEnumerator MoveObject(GameObject obj)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = startPosition + moveDirection.normalized * moveDistance;

        while (Vector3.Distance(obj.transform.position, endPosition) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, endPosition, Time.deltaTime * 5f); // 5f는 이동 속도
            yield return null;
        }

        // 오브젝트를 일정 시간 후에 반환
        yield return new WaitForSeconds(2f);
        objectPool.ReturnObject(obj);
    }
}