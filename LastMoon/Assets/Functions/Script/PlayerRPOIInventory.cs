using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPOIInventory : MonoBehaviour
{
    private Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory();
    }

    public void AddItem(ScriptableObject_Item itemType, int count)
    {
        bool itemExists = false;
        foreach (Item item in inventory.GetItems())
        {
            if (item.ItemType == itemType)
            {
                int remainCount = item.AddItem(itemType, count);
                if (remainCount > 0)
                {
                    // 아이템이 최대치를 초과하면, 나머지 아이템은 새로운 아이템으로 추가
                    inventory.AddItem(new Item { ItemType = itemType, Count = remainCount });
                }
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {
            // 인벤토리에 해당 아이템이 없으면 새로 추가
            Item newItem = new Item { ItemType = itemType, Count = count };
            inventory.AddItem(newItem);
        }

        Debug.Log(itemType.ItemName + " added to inventory. Current count: " + count);
    }
}
