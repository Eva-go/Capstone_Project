using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Button_Use : MonoBehaviour
{
    private PlayerController playerController;
    public ScriptableObject_Item ItemType;

    public void RegisterPlayerController(PlayerController player)
    {
        playerController = player;
    }

    public void Use_Item()
    {
        switch (ItemType.ConsumableType)
        {
            case 0:
                Use_Item_Healing();
                break;
        }
    }

    public void Use_Item_Healing()
    {
        //playerController.heal(ItemType.ConsumeStrength);
        playerController.pv.RPC("RPC_RecoverHp", RpcTarget.All, ItemType.ConsumeStrength);
        playerController.PlayerInventory.RemoveItem(new Item { Count = 1, ItemType = ItemType });
    }
}
