using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (item.Count > 0)
        {
            bool AlreadyHas = false;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.ItemType == item.ItemType)
                {
                    inventoryItem.Count += item.Count;
                    AlreadyHas = true;
                }
            }
            if (!AlreadyHas) itemList.Add(item);
        }
    }

    public bool RemoveItem(Item item)
    {
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.ItemType == item.ItemType)
            {
                if (inventoryItem.Count - item.Count >= 0)
                {
                    inventoryItem.Count -= item.Count;
                    if (inventoryItem.Count == 0) itemList.Remove(inventoryItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    public int ClearItem(ScriptableObject_Item itemType)
    {
        int ItemCount = 0;
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.ItemType == itemType)
            {
                ItemCount += inventoryItem.Count;
                itemList.Remove(inventoryItem);
            }
        }
        return ItemCount;
    }

    public int SellAllItem()
    {
        int ItemCount = 0;
        foreach (Item inventoryItem in itemList)
        {
            ItemCount += inventoryItem.Count * inventoryItem.ItemType.Price;
            itemList.Remove(inventoryItem);
        }
        return ItemCount;
    }

    public List<Item> GetItems()
    {
        return itemList;
    }
}
