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
            case 1:
                Use_Item_Recipe_Unlock();
                break;
        }
    }

    public void Use_Item_Healing()
    {
        //playerController.heal(ItemType.ConsumeStrength);
        playerController.pv.RPC("RPC_RecoverHp", RpcTarget.All, ItemType.ConsumeStrength);
        playerController.PlayerInventory.RemoveItem(new Item { Count = 1, ItemType = ItemType });
    }

    public void Use_Item_Recipe_Unlock()
    {
        for (int i = 0; i < playerController.UnlockedRecipe.Length; i++)
        {
            if (!playerController.UnlockedRecipe[i])
            {
                playerController.UnlockedRecipe[i] = true;
                playerController.PlayerInventory.RemoveItem(new Item { Count = 1, ItemType = ItemType });
                break;
            }
        }
        /*
        if (playerController.UnlockedRecipe[0])
        {
            for (int i = 1; i < playerController.UnlockedRecipe.Length; i++)
            {
                if (!playerController.UnlockedRecipe[i])
                {
                    playerController.UnlockedRecipe[i] = true;
                    playerController.PlayerInventory.RemoveItem(new Item { Count = 1, ItemType = ItemType });
                    break;
                }
            }
            //float RandomRecipe = Random.value;
            //float Probability = 1f / (playerController.UnlockedRecipe.Length - 1f);
            //for (int i = 1; i < playerController.UnlockedRecipe.Length; i++)
            //{
            //    if (RandomRecipe > Probability * i
            //        && Probability * (i + 1) > RandomRecipe)
            //        playerController.UnlockedRecipe[i] = true;
            //}
        }
        else
        {
            playerController.UnlockedRecipe[0] = true;
            playerController.PlayerInventory.RemoveItem(new Item { Count = 1, ItemType = ItemType });
        }*/
    }
}
