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
        /*
        foreach(Item inventoryItem in itemList)
        {
            bool AlreadyHas = false;
            if (inventoryItem.ItemType == item.ItemType)
            {
                inventoryItem.Count += item.Count;
                AlreadyHas = true;
            }
            if (!AlreadyHas) itemList.Add(item);
        }
         */

        itemList.Add(item);
    }
    public List<Item> GetItems()
    {
        return itemList;
    }
}
