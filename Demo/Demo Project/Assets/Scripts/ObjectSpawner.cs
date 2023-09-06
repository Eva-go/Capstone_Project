using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // 스폰할 오브젝트 프리팹
    public int numberOfObjects = 10; // 생성할 오브젝트 개수
    public Vector3 spawnAreaCenter; // 스폰 영역 중심 위치
    public Vector3 spawnAreaSize; // 스폰 영역 크기
    public float minObjectSize = 0.5f; // 오브젝트 최소 크기
    public float maxObjectSize = 2.0f; // 오브젝트 최대 크기
    public int seed = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        Random.InitState(SeedGenerator()); // 시드 값을 기반으로 랜덤 초기화

        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnObject();
        }
    }

    private int SeedGenerator()
    {
        // 현재 시간을 시드 값으로 사용
        //System.DateTime now = System.DateTime.Now;
        //int seed = now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond;
        return seed;
    }

    private void SpawnObject()
    {
        // 스폰 위치 랜덤하게 설정
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
            Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
            Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
        );

        // 스폰 크기 랜덤하게 설정
        float randomSize = Random.Range(minObjectSize, maxObjectSize);

        // 오브젝트 생성 및 크기 설정
        GameObject newObject = Instantiate(objectPrefab, randomPosition, Quaternion.identity);
        newObject.transform.localScale = Vector3.one * randomSize;

        // 다른 오브젝트들과의 충돌 체크
        bool isColliding = false;
        Collider newObjectCollider = newObject.GetComponent<Collider>();
        foreach (GameObject existingObject in spawnedObjects)
        {
            Collider existingObjectCollider = existingObject.GetComponent<Collider>();

            if (newObjectCollider.bounds.Intersects(existingObjectCollider.bounds))
            {
                isColliding = true;
                break;
            }
        }

        if (!isColliding)
        {
            spawnedObjects.Add(newObject);
        }
        else
        {
            Destroy(newObject);
        }
    }
}