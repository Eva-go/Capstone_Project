using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트가 씬 변경 시에도 파괴되지 않도록 함
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spawnPoints = GameObject.Find("APTSpawner").GetComponentsInChildren<Transform>(); // 스폰 포인트 가져오기
        if (spawnPoints.Length > 1)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogError("스폰 포인트가 없거나 충분하지 않습니다.");
        }
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log("Waiting before respawning...");
        yield return new WaitForSeconds(2.0f); // 2초 대기 (필요에 따라 변경 가능)

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        int idx = Random.Range(1, spawnPoints.Length);
        Debug.Log("Spawning player at point: " + idx);

        GameObject player = PhotonNetwork.Instantiate("Player", spawnPoints[idx].position, spawnPoints[idx].rotation, 0);
        if (player != null)
        {
            player.name = GameValue.nickNumae;
            Transform OtherPlayer = player.transform.Find("OtherPlayer");
            Transform LocalPlayer = player.transform.Find("LocalPlayer");
            Transform Tool = player.transform.Find("Player001");
            Transform T_LocalPlayerTool = player.transform.Find("ToolCamera");
            OtherPlayer.gameObject.SetActive(false);
            LocalPlayer.gameObject.SetActive(true);
            Tool.gameObject.SetActive(false);
            T_LocalPlayerTool.gameObject.SetActive(true);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            Debug.Log("Player spawned and ownership transferred.");
            PlayerController.Hp = 100;
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}