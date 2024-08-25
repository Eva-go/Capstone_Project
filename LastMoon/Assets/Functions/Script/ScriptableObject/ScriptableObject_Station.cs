using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Station")]
public class ScriptableObject_Station : ScriptableObject
{
    public ScriptableObject_Item[] Input = new ScriptableObject_Item[3];
    public int InputCount;

    public ScriptableObject_Item[] Output = new ScriptableObject_Item[3];
    public int OutputCount;

    public float Temperture;
    public float Coolent;

    public float ProgressTime;
}
