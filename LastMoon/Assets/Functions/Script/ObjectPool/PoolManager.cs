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
            // �����̽��ٸ� ������ Ǯ���� ������Ʈ�� ������
            GameObject obj = objectPool.GetObject();
            obj.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);

            // ���� �ð� �Ŀ� ������Ʈ�� ��ȯ��
            StartCoroutine(ReturnObjectAfterTime(obj, 2f));
        }
    }

    private IEnumerator ReturnObjectAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectPool.ReturnObject(obj);
    }
}