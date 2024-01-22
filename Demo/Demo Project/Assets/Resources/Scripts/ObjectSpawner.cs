using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // ������ ������Ʈ ������
    public int numberOfObjects = 10; // ������ ������Ʈ ����
    public Vector3 spawnAreaCenter; // ���� ���� �߽� ��ġ
    public Vector3 spawnAreaSize; // ���� ���� ũ��
    public float minObjectSize = 0.5f; // ������Ʈ �ּ� ũ��
    public float maxObjectSize = 2.0f; // ������Ʈ �ִ� ũ��
    public int seed = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        Random.InitState(SeedGenerator()); // �õ� ���� ������� ���� �ʱ�ȭ

        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnObject();
        }
    }

    private int SeedGenerator()
    {
        // ���� �ð��� �õ� ������ ���
        //System.DateTime now = System.DateTime.Now;
        //int seed = now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond;
        return seed;
    }

    private void SpawnObject()
    {
        // ���� ��ġ �����ϰ� ����
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
            Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
            Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
        );

        // ���� ũ�� �����ϰ� ����
        float randomSize = Random.Range(minObjectSize, maxObjectSize);

        // ������Ʈ ���� �� ũ�� ����
        GameObject newObject = Instantiate(objectPrefab, randomPosition, Quaternion.identity);
        newObject.transform.localScale = Vector3.one * randomSize;

        // �ٸ� ������Ʈ����� �浹 üũ
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