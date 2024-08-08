using UnityEngine;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public ObjectPool objectPool;
    public Vector3 moveDirection = Vector3.right; // �̵� ����
    public float moveDistance = 10f; // �̵� �Ÿ�

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // �����̽��ٸ� ������ Ǯ���� ������Ʈ�� ������
            GameObject obj = objectPool.GetObject();
            obj.transform.position = Vector3.zero; // ���� ��ġ ����
            StartCoroutine(MoveObject(obj));
        }
    }

    private IEnumerator MoveObject(GameObject obj)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = startPosition + moveDirection.normalized * moveDistance;

        while (Vector3.Distance(obj.transform.position, endPosition) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, endPosition, Time.deltaTime * 5f); // 5f�� �̵� �ӵ�
            yield return null;
        }

        // ������Ʈ�� ���� �ð� �Ŀ� ��ȯ
        yield return new WaitForSeconds(2f);
        objectPool.ReturnObject(obj);
    }
}