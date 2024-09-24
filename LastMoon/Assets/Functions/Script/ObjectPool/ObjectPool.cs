using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class ObjectPool : MonoBehaviour
{
    private GameObject prefab;
    private Queue<GameObject> pool;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            obj.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Ǯ�� ��ü�� ���� ��� ���ο� ��ü�� ������ �� ����
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            return obj;
        }
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}