using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Station")]
public class ScriptableObject_Station : ScriptableObject
{
    public ScriptableObject_Item Output001;
    public ScriptableObject_Item Output002;
    public ScriptableObject_Item Output003;

    public ScriptableObject_Item Input001;
    public ScriptableObject_Item Input002;
    public ScriptableObject_Item Input003;

    public int Temperture;
    public int Coolent;

    public int ProgressTime;
}
