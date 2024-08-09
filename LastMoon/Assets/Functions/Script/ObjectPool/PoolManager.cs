using UnityEngine;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public ObjectPool objectPool; // 물방울 풀
    public Transform spawnPoint; // 물방울이 떨어질 위치

    private void Start()
    {
        if (objectPool == null)
        {
            Debug.LogError("objectPool is not assigned in the PoolManager script.");
            return;
        }

        // 주기적으로 물방울 떨어뜨리기
        StartCoroutine(DropWaterDrops());
    }

    //private IEnumerator DropWaterDrops()
    //{
    //    while (true)
    //    {
    //        GameObject objectPools = objectPool.GetObject();
    //        if (objectPools != null)
    //        {
    //            objectPools.transform.position = spawnPoint.position;
    //            objectPools.transform.rotation = Quaternion.identity; // 기본 회전
    //            objectPools.SetActive(true);
    //
    //            // 물방울이 떨어진 후 풀에 반환
    //            yield return new WaitForSeconds(2f); // 물방울이 2초 동안 보이게 한다고 가정
    //            objectPool.ReturnObject(objectPools);
    //        }
    //
    //        yield return new WaitForSeconds(2f);
    //    }
    //}
    private IEnumerator DropWaterDrops()
    {
        while (false) // 무한 루프를 잠시 비활성화
        {
            yield return null;
        }
    }
}