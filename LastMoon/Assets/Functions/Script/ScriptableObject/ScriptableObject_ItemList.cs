using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_ItemList")]
public class ScriptableObject_ItemList : ScriptableObject
{
    public ScriptableObject_Item[] Items;
    public ScriptableObject_Item NullItem;

    public ScriptableObject_Item FindListByIDMatch(string ItemID)
    {
        for (int i = 0; i < Items.Length; i++) 
        {
            if (Items[i].ItemName == ItemID)
            {
                return Items[i];
            }
        }
        return NullItem;
    }
}
