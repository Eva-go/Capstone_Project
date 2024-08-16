using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Item")]
public class ScriptableObject_Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;

    public Texture ItemLU;
    public Material ItemLUM;
    public Material ItemLUT;

    public GameObject ItemObject;

    public int Price;
    public int MaxCount;

    public float Heating;
}
