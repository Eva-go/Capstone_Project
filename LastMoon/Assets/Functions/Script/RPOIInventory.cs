using System.Collections.Generic;
using UnityEngine;

public class RPOIInventory : MonoBehaviour
{
    public ScriptableObject_Item[] RPOI_Items; // RPOI에 지정된 아이템 배열
    public int[] ItemCounts; // 각 아이템의 개수 배열

    public Inventory inventory; // RPOI의 자체 인벤토리

    private void Awake()
    {
        inventory = new Inventory(); // 인벤토리 초기화
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
                    inventory.AddItem(new Item { ItemType = itemType, Count = remainCount });
                }
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {
            Item newItem = new Item { ItemType = itemType, Count = count };
            inventory.AddItem(newItem);
        }

        Debug.Log(itemType.ItemName + " added to RPOI inventory. Current count: " + count);
    }
}
