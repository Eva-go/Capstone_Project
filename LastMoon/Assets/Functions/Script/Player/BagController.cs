using Photon.Pun;
using UnityEngine;

public class BagController : MonoBehaviourPunCallbacks
{
    public int[] nodeItems = new int[6];
    public int[] mixItems = new int[6];
    
    public float health = 10f;
    public Inventory BagInventory = new Inventory { };
    public ScriptableObject_ItemList Itemlist;

    private int lastAttackerViewID;

    [PunRPC]
    public void GetItem(string ItemType, int ItemCount)
    {
        BagInventory.AddItem(new Item { ItemType = Itemlist.FindListByIDMatch(ItemType), Count = ItemCount });
        if (health < 100)
        {
            health += (int)(ItemCount / 10);
        }
        else if (health != 100) health = 100;
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
            }
        }
        if (PhotonNetwork.IsMasterClient || photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}