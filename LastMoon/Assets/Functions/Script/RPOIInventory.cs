using System.Collections.Generic;
using UnityEngine;

public class RPOIInventory : MonoBehaviour
{
    public ScriptableObject_Item[] RPOI_Items; // RPOI�� ������ ������ �迭
    public int[] ItemCounts; // �� �������� ���� �迭

    public Inventory inventory; // RPOI�� ��ü �κ��丮

    private void Awake()
    {
        inventory = new Inventory(); // �κ��丮 �ʱ�ȭ
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
