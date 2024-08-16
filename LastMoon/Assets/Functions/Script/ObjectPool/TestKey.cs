using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKey : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 1f; // 스폰 간격
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

            // 오브젝트를 특정 위치에 스폰
            Vector3 spawnPosition = transform.position; // 예를 들어 현재 위치
            ObjectPooler.SpawnFromPool<TestPooler>("Sphere", spawnPosition).Setup(true);
        }
    }
}