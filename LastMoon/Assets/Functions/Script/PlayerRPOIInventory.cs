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
                    // �������� �ִ�ġ�� �ʰ��ϸ�, ������ �������� ���ο� ���������� �߰�
                    inventory.AddItem(new Item { ItemType = itemType, Count = remainCount });
                }
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {
            // �κ��丮�� �ش� �������� ������ ���� �߰�
            Item newItem = new Item { ItemType = itemType, Count = count };
            inventory.AddItem(newItem);
        }

        Debug.Log(itemType.ItemName + " added to inventory. Current count: " + count);
    }
}
