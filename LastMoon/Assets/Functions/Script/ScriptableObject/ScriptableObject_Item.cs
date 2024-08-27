using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Item")]
public class ScriptableObject_Item : ScriptableObject
{
    public string ItemName = "Empty";
    public Sprite ItemSprite;

    public Texture ItemLU;
    public Material ItemLUM;
    public Material ItemLUT;

    public GameObject ItemObject;

    public int ItemType; // 0 - misc, 1 - construction, 2 - consumable
    public int ConsumableType; // 0 - healing
    public float ConsumeStrength;

    public int Price;
    public int MaxCount;

    public float Heating;
}
