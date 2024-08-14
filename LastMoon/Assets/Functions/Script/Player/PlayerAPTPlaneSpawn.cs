using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAPTPlaneSpawn: MonoBehaviour
{
    public PlayerController player;

    public GameObject APTPlane;
    private float SpawnPos;
    private Vector3 spawnPoint;
    public Vector3 playerPoint;
    public Vector3 playerrotation;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPos = 100;
        spawnPoint = new Vector3((GameValue.PlayerID + 1) * SpawnPos, SpawnPos, (GameValue.PlayerID + 1) * SpawnPos);
        GameObject instance = Instantiate(APTPlane, spawnPoint, Quaternion.identity);
        playerPoint = new Vector3((GameValue.PlayerID + 1) * SpawnPos, SpawnPos, (GameValue.PlayerID + 1) * SpawnPos + 2.5f);
        playerrotation = new Vector3(gameObject.transform.rotation.x, -gameObject.transform.rotation.y, gameObject.transform.rotation.z);
        player= gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

    }
}
