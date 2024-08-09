using UnityEngine;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public ObjectPool objectPool; // ����� Ǯ
    public Transform spawnPoint; // ������� ������ ��ġ

    private void Start()
    {
        if (objectPool == null)
        {
            Debug.LogError("objectPool is not assigned in the PoolManager script.");
            return;
        }

        // �ֱ������� ����� ����߸���
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
    //            objectPools.transform.rotation = Quaternion.identity; // �⺻ ȸ��
    //            objectPools.SetActive(true);
    //
    //            // ������� ������ �� Ǯ�� ��ȯ
    //            yield return new WaitForSeconds(2f); // ������� 2�� ���� ���̰� �Ѵٰ� ����
    //            objectPool.ReturnObject(objectPools);
    //        }
    //
    //        yield return new WaitForSeconds(2f);
    //    }
    //}
    private IEnumerator DropWaterDrops()
    {
        while (false) // ���� ������ ��� ��Ȱ��ȭ
        {
            yield return null;
        }
    }
}