using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectPrefab; // 풀링할 오브젝트의 프리팹
    public int poolSize = 10; // 풀의 크기

    private Queue<GameObject> poolQueue; // 오브젝트를 저장할 큐

    private void Awake()
    {
        // 큐 초기화
        poolQueue = new Queue<GameObject>();

        // 오브젝트 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    // 풀에서 오브젝트 가져오기
    public GameObject GetObject()
    {
        if (poolQueue.Count > 0)
        {
            GameObject obj = poolQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // 큐에 오브젝트가 없으면 새로운 오브젝트 생성
            GameObject obj = Instantiate(objectPrefab);
            return obj;
        }
    }

    // 풀에 오브젝트 반환하기
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}