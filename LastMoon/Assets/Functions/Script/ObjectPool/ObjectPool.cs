using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectPrefab; // Ǯ���� ������Ʈ�� ������
    public int poolSize = 10; // Ǯ�� ũ��

    private Queue<GameObject> poolQueue; // ������Ʈ�� ������ ť

    private void Awake()
    {
        // ť �ʱ�ȭ
        poolQueue = new Queue<GameObject>();

        // ������Ʈ �̸� ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    // Ǯ���� ������Ʈ ��������
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
            // ť�� ������Ʈ�� ������ ���ο� ������Ʈ ����
            GameObject obj = Instantiate(objectPrefab);
            return obj;
        }
    }

    // Ǯ�� ������Ʈ ��ȯ�ϱ�
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}