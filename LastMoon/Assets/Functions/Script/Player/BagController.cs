using Photon.Pun;
using UnityEngine;

public class BagController : MonoBehaviourPunCallbacks
{
    public int[] nodeItems = new int[6];
    public int[] mixItems = new int[6];
    public float health = 30f;

    private int lastAttackerViewID;

    [PunRPC]
    public void GetItme(int node, int mix, int index)
    {
        if (index >= 0 && index < nodeItems.Length)
        {
            nodeItems[index] = node;
            mixItems[index] = mix;
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerViewID)
    {
        health -= damage;
        lastAttackerViewID = attackerViewID;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PhotonView attackerPhotonView = PhotonView.Find(lastAttackerViewID);
        if (attackerPhotonView != null)
        {
            PlayerController playerController = attackerPhotonView.GetComponent<PlayerController>();
            if (playerController != null)
            {
                for (int i = 0; i < nodeItems.Length; i++)
                {
                    playerController.nodeItiems[i] += nodeItems[i];
                }
                for (int i = 0; i < mixItems.Length; i++)
                {
                    playerController.mixItiems[i] += mixItems[i];
                }

                // 인벤토리 변경 이벤트 호출
                playerController.InvokeInventoryChanged();
            }
        }
        if (PhotonNetwork.IsMasterClient || photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}