using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolManager : MonoBehaviour
{
    public ObjectPool objectPool;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 스페이스바를 누르면 풀에서 오브젝트를 가져옴
            GameObject obj = objectPool.GetObject();
            obj.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);

            // 일정 시간 후에 오브젝트를 반환함
            StartCoroutine(ReturnObjectAfterTime(obj, 2f));
        }
    }

    private IEnumerator ReturnObjectAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectPool.ReturnObject(obj);
    }
}