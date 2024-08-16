using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKey : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 1f; // ���� ����
    private float lastSpawnTime;

    void Start()
    {
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Time.time - lastSpawnTime >= spawnInterval)
        {
            lastSpawnTime = Time.time;

            // ������Ʈ�� Ư�� ��ġ�� ����
            Vector3 spawnPosition = transform.position; // ���� ��� ���� ��ġ
            ObjectPooler.SpawnFromPool<TestPooler>("Sphere", spawnPosition).Setup(true);
        }
    }
}