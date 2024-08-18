using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory UIinventory;
    private Transform ItemTab;
    private Transform ItemSlot;

    private void Awake()
    {
        ItemTab = transform.Find("Misc_Tab");
        ItemSlot = ItemTab.Find("Item_Slot");
    }

    public void SetInventory(Inventory inventory)
    {
        UIinventory = inventory;
    }
    private void RefreshUI()
    {
        int x = 0;
        int y = 0;
        float itemSlotSize = 150f;
        foreach (Item item in UIinventory.GetItems())
        {
            RectTransform itemRectTransform = Instantiate(ItemSlot, ItemTab).GetComponent<RectTransform>();
            itemRectTransform.gameObject.SetActive(true);
            itemRectTransform.anchoredPosition = new Vector2(x * itemSlotSize, y * itemSlotSize);

            Image image = itemRectTransform.Find("Icon").GetComponent<Image>();
            image.sprite = item.ItemType.ItemSprite;


            y++;
        }
    }
}
