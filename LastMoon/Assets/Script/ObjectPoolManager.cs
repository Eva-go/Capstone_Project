using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;

    public int poolCount = 10; // 총알 갯수
    List<GameObject> bullets; // Object Pool

    void Start()
    {
        bullets = new List<GameObject>();
        while (poolCount > 0)
        {
            GameObject obj = Instantiate(bullet, firePos.position, Quaternion.identity);
            obj.SetActive(false); // 생성되자마자 비활성화
            bullets.Add(obj); // Pool 리스트에 추가
            poolCount -= 1;
        }
        StartCoroutine(Fire(0.1f)); // 0.1의 시간간격을 두고 실행
    }
    private IEnumerator Fire(float coolTime)
    {
        while (true)
        {
            foreach (GameObject bullet in bullets)
            { // Pool을 뒤져서
                if (!bullet.activeInHierarchy)
                { // 비활성화된 오브젝트가 있다면
                    bullet.transform.position = firePos.position; // firePos 위치에
                    bullet.GetComponent<BulletBehaviour>().Spawn(); // 탄 생성!
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
    { // 활성화 된다면
        StartCoroutine(ActiveTime(3f));
    }

    public void Spawn()
    {
        gameObject.SetActive(true); // 생성 = 활성화
    }

    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSeconds(coolTime); // coolTime만큼 활성화
        gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
