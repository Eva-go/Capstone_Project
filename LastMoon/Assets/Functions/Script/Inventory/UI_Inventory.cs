using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory UIinventory;
    private Transform ItemSlot;
    private Transform ItemSlotTemplate;


    private void Awake()
    {
        ItemSlot = transform.Find("");
        ItemSlotTemplate = ItemSlot.Find("");
    }

    public void SetInventory(Inventory inventory)
    {
        UIinventory = inventory;
    }


    private void RefreshUI()
    {
        int x = 0;
        int y = 0;
        float itemSlotSize = 30f;
        foreach (Item item in UIinventory.GetItems())
        {
            RectTransform itemRectTransform = Instantiate(ItemSlotTemplate, ItemSlot).GetComponent<RectTransform>();
            itemRectTransform.gameObject.SetActive(true);
            itemRectTransform.anchoredPosition = new Vector2(x * itemSlotSize, y * itemSlotSize);
            y++;
        }
    }
}
