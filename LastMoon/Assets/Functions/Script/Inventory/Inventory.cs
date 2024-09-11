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
            if (!AlreadyHas) itemList.Add(new Item { ItemType = item.ItemType, Count = item.Count });
        }
    }

    public bool RemoveItem(Item item)
    {
        Item SelectedItem = null;
        bool RemovedItem = false;
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.ItemType == item.ItemType)
            {
                if (inventoryItem.Count - item.Count >= 0)
                {
                    inventoryItem.Count -= item.Count;
                    if (inventoryItem.Count == 0) SelectedItem = inventoryItem;
                    RemovedItem = true;
                }
            }
        }
        if (SelectedItem != null) 
        {
            itemList.Remove(SelectedItem);
        }
        return RemovedItem;
    }

    public bool CheckItem(Item item)
    {
        bool HasItem = false;
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.ItemType == item.ItemType)
            {
                if (inventoryItem.Count >= item.Count)
                {
                    HasItem = true;
                }
            }
        }
        return HasItem;
    }

    public int ClearItem(ScriptableObject_Item itemType)
    {
        Item SelectedItem = null;
        int ItemCount = 0;
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.ItemType == itemType)
            {
                ItemCount += inventoryItem.Count;
                SelectedItem = inventoryItem;
            }
        }
        if (SelectedItem != null)
        {
            itemList.Remove(SelectedItem);
        }
        return ItemCount;
    }

    public void ClearInventory()
    {
        itemList.Clear();
    }
    public void OverrideInventory(Inventory inventory)
    {
        itemList = inventory.GetItems();
    }


    public int SellAllItem()
    {
        int ItemCount = 0;
        foreach (Item inventoryItem in itemList)
        {
            ItemCount += inventoryItem.Count * inventoryItem.ItemType.Price;
        }
        itemList.Clear();
        return ItemCount;
    }

    public List<Item> GetItems()
    {
        return itemList;
    }
}
