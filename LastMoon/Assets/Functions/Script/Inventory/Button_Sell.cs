using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Sell : MonoBehaviour
{
    private PlayerController playerController;
    private int TotalSell = 0;
    public ScriptableObject_Item ItemType;

    public void RegisterPlayerController(PlayerController player)
    {
        playerController = player;
    }

    public void Sell_Item()
    {
        int ItemCount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) ItemCount *= 10;
        if (Input.GetKey(KeyCode.LeftControl)) ItemCount *= 5;

        if (ItemType != null)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
                TotalSell = playerController.Sell_ItemStack(ItemType);
            else
                TotalSell = playerController.Sell_Item(ItemType, ItemCount);
            GameValue.GetMomey(TotalSell);
        }
    }
}
