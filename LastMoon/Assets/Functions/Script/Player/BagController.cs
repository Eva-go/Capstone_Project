using Photon.Pun;
using UnityEngine;

public class BagController : MonoBehaviourPunCallbacks
{
    public int[] nodeItems = new int[6];
    public int[] mixItems = new int[6];
    
    public float health = 30f;
    public Inventory BagInventory = new Inventory { };

    private int lastAttackerViewID;

    [PunRPC]
    public void GetItem(int ItemCount)
    {
        //BagInventory.OverrideInventory(inventory);
        /*
        foreach (Item item in inventory.GetItems())
        {
            BagInventory.AddItem(item);
            health += item.Count * 5;
        }
         */
        //BagInventory.AddItem(item);
        //health += item.Count * 5;
        health += ItemCount;
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
                foreach (Item item in BagInventory.GetItems())
                {
                    playerController.PlayerInventory.AddItem(item);
                    playerController.TakeDamage(5f);
                }
                /*
                for (int i = 0; i < nodeItems.Length; i++)
                {
                    playerController.nodeItiems[i] += nodeItems[i];
                }
                for (int i = 0; i < mixItems.Length; i++)
                {
                    playerController.mixItiems[i] += mixItems[i];
                }
                 */

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