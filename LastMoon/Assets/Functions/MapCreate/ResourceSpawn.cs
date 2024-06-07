using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawn : MonoBehaviour
{
    [Header("Spawn Setting")]
    public GameObject spawnPrefab;
    public float spawnChange;

    [Header("Raycast Setting")]
    public float distanceBetweenChecks;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;

    private void Start()
    {
        SpawnResource();
    }

    void SpawnResource()
    {
        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenChecks)
        {
            for (float z = positivePosition.y; z < negativePosition.y; z += distanceBetweenChecks)
            {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, heightOfCheck, z),Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if(spawnChange > Random.Range(0, 101)){
                        Instantiate(spawnPrefab, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0,360),0)),transform);
                    }
                }
            }
        }
    }
}
