using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public ScriptableObject_Item itemData;
    public int itemCount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRPOIInventory playerInventory = other.GetComponent<PlayerRPOIInventory>();

            // ���� �÷��̾ PlayerInventory�� ���ٸ� �߰�
            if (playerInventory == null)
            {
                playerInventory = other.gameObject.AddComponent<PlayerRPOIInventory>();
            }

            playerInventory.AddItem(itemData, itemCount);
        }
    }
}
