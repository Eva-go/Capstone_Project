using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectPrefab; // 풀링할 오브젝트의 프리팹
    public int poolSize = 10; // 풀의 크기

    private Queue<GameObject> poolQueue; // 오브젝트를 저장할 큐

    private void Awake()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("Object Prefab is not assigned in the ObjectPool script.");
            return;
        }

        poolQueue = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
            Debug.Log("Created and added object to pool: " + obj.name);
        }
    }

    
    public GameObject GetObject()
    {
        if (poolQueue == null)
        {
            Debug.LogError("PoolQueue is not initialized.");
            return null;
        }

        if (poolQueue.Count > 0)
        {
            GameObject obj = poolQueue.Dequeue();
            obj.SetActive(true);
            Debug.Log("Retrieved object from pool: " + obj.name);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(objectPrefab);
            Debug.Log("Instantiated new object: " + obj.name);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("Trying to return a null object.");
            return;
        }

        obj.SetActive(false);

        if (poolQueue != null)
        {
            poolQueue.Enqueue(obj);
            Debug.Log("Returned object to pool: " + obj.name);
        }
        else
        {
            Debug.LogError("PoolQueue is not initialized.");
        }
    }
}