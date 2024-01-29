using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;

    public int poolCount = 10; // �Ѿ� ����
    List<GameObject> bullets; // Object Pool

    void Start()
    {
        bullets = new List<GameObject>();
        while (poolCount > 0)
        {
            GameObject obj = Instantiate(bullet, firePos.position, Quaternion.identity);
            obj.SetActive(false); // �������ڸ��� ��Ȱ��ȭ
            bullets.Add(obj); // Pool ����Ʈ�� �߰�
            poolCount -= 1;
        }
        StartCoroutine(Fire(0.1f)); // 0.1�� �ð������� �ΰ� ����
    }
    private IEnumerator Fire(float coolTime)
    {
        while (true)
        {
            foreach (GameObject bullet in bullets)
            { // Pool�� ������
                if (!bullet.activeInHierarchy)
                { // ��Ȱ��ȭ�� ������Ʈ�� �ִٸ�
                    bullet.transform.position = firePos.position; // firePos ��ġ��
                    bullet.GetComponent<BulletBehaviour>().Spawn(); // ź ����!
                    break;
                }
            }
            yield return new WaitForSeconds(coolTime);
        }
    }
}

public class BulletBehaviour : MonoBehaviour
{
    public float speed = 5f;

    private void OnEnable()
    { // Ȱ��ȭ �ȴٸ�
        StartCoroutine(ActiveTime(3f));
    }

    public void Spawn()
    {
        gameObject.SetActive(true); // ���� = Ȱ��ȭ
    }

    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSeconds(coolTime); // coolTime��ŭ Ȱ��ȭ
        gameObject.SetActive(false); // coolTime �� ������ ��Ȱ��ȭ
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
