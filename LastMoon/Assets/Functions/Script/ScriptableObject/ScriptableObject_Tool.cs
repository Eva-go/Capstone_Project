using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Tool")]
public class ScriptableObject_Tool : ScriptableObject
{
    public string ToolName;
    public Sprite ToolSprite;
    public GameObject ToolObject;

    public ScriptableObject_Item Tool_Head;
    public ScriptableObject_Item Tool_Handle;

    public int Price;

    public float Damage;
    public float HarvestPower;
    public float Knockback;
    public float Speed;
}
