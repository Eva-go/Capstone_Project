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
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ���� �ÿ��� �ı����� �ʵ��� ��
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spawnPoints = GameObject.Find("APTSpawner").GetComponentsInChildren<Transform>(); // ���� ����Ʈ ��������
        if (spawnPoints.Length > 1)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogError("���� ����Ʈ�� ���ų� ������� �ʽ��ϴ�.");
        }
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log("Waiting before respawning...");
        yield return new WaitForSeconds(2.0f); // 2�� ��� (�ʿ信 ���� ���� ����)

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