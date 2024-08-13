using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ScriptableObject_Item ItemType;
    public int Count;

    public int AddItem(ScriptableObject_Item AddType, int AddCount)
    {
        if (ItemType == AddType)
        {
            Count += AddCount;
        }
        else return -1;

        if (Count > AddType.MaxCount)
        {
            int RemainCount = Count - AddType.MaxCount;
            Count = AddType.MaxCount;
            return RemainCount;
        }
        else return 0;
    }

    public void SubtractItem(int SubCount)
    {
        Count -= SubCount;
        if (Count < 0) Count = 0;
    }
}
