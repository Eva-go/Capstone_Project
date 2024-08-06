using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAPTPlaneSpawn: MonoBehaviour
{
    public GameObject APTPlane;
    private float SpawnPos;
    private Vector3 spawnPoint;
    private Vector3 playerPoint;
    private bool Isinside;
    // Start is called before the first frame update
    void Start()
    {
        Isinside = false;
        SpawnPos = 100;
        spawnPoint = new Vector3((GameValue.PlayerID + 1) * SpawnPos, SpawnPos, (GameValue.PlayerID + 1) * SpawnPos);
        GameObject instance = Instantiate(APTPlane, spawnPoint, Quaternion.identity);
        playerPoint = new Vector3((GameValue.PlayerID + 1) * SpawnPos, SpawnPos, (GameValue.PlayerID + 1) * SpawnPos + 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameValue.inside&& !Isinside)
        {
            gameObject.transform.position = playerPoint;
            Isinside = true;
        }
    }
}
