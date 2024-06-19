using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RespawnPlayer(Transform[] spawnPoints)
    {
        StartCoroutine(RespawnCoroutine(spawnPoints));
    }

    private IEnumerator RespawnCoroutine(Transform[] spawnPoints)
    {
        Debug.Log("Waiting before respawning...");
        yield return new WaitForSeconds(2.0f); // 2초 대기 (필요에 따라 변경 가능)

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