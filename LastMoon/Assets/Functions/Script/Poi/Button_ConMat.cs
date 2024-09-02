using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_ConMat : MonoBehaviour
{
    public GameObject ItemTab;
    public GameObject Subtab;

    public GameObject[] ItemSelect;
    public Button[] ItemSelect_BT;
    public ScriptableObject_Item[] SelectableItems;

    public GameObject[] SubTabItemSelect;
    public Button[] SubTabItemSelect_BT;
    public ScriptableObject_Item[] SubTabSelectableItems;

    public bool hasSubtab;
    public ScriptableObject_Item SelectedItem;

    public PlayerPoiSpawn Station_Construction_Master;

    public bool Opened;

    public void OpenTab()
    {
        if (!ItemTab.activeSelf)
        {
            Opened = true;
            ItemTab.SetActive(true);
        }
        else
        {
            if (Opened)
            {
                ItemTab.SetActive(false);
                Station_Construction_Master.BtnConMat_CloseAllTab();
            }
            else
            {
                Station_Construction_Master.BtnConMat_CloseAllTab();
                Opened = true;
            }
        }

        ItemTab.transform.GetChild(1).gameObject.SetActive(false);
        ItemTab.transform.GetChild(2).gameObject.SetActive(false);

        if (hasSubtab && Subtab != null)
        {
            Subtab.SetActive(true);
            for (int i = 0; i < SubTabItemSelect.Length; i++)
            {
                SubTabItemSelect[i] = Subtab.transform.GetChild(i + 1).gameObject;

                ScriptableObject_Item ItemType = SubTabSelectableItems[i];
                SubTabItemSelect_BT[i] = SubTabItemSelect[i].GetComponent<Button>();
                SubTabItemSelect_BT[i].onClick.RemoveAllListeners();
                SubTabItemSelect_BT[i].onClick.AddListener(() => SelectItem(ItemType));

                Image image = SubTabItemSelect[i].transform.GetChild(0).GetComponent<Image>();
                image.sprite = SubTabSelectableItems[i].ItemSprite;
            }
        }

        for (int i = 0; i < ItemSelect.Length; i++)
        {
            ItemSelect[i] = ItemTab.transform.GetChild(3).GetChild(i).gameObject;

            ScriptableObject_Item ItemType = SelectableItems[i];
            ItemSelect_BT[i] = ItemSelect[i].GetComponent<Button>();
            ItemSelect_BT[i].onClick.RemoveAllListeners();
            ItemSelect_BT[i].onClick.AddListener(() => SelectItem(ItemType));

            Image image = ItemSelect[i].transform.GetChild(0).GetComponent<Image>();
            image.sprite = SelectableItems[i].ItemSprite;
        }

    }

    public void SelectItem(ScriptableObject_Item Type)
    {
        SelectedItem = Type;
        Image image = gameObject.transform.GetChild(0).GetComponent<Image>();
        image.sprite = SelectedItem.ItemSprite;

        if (Station_Construction_Master != null)
        {
            Station_Construction_Master.GetConMatItem();
        }
    }
}
