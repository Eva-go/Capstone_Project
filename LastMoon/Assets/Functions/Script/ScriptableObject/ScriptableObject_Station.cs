using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ScriptableObject_Station")]
public class ScriptableObject_Station : ScriptableObject
{
    public ScriptableObject_Recipe[] SelectableRecipes;

    public int StationMaterialCount;
    public int[] StationMatType; // 0 - Default, 1 - Input, 2 - Output, 3 - InOut, 4 - Fuel, 5 - Coolent

    public bool[] TempertureSensitive;

    public bool StationAux;
    public bool StationFix;
    public bool StationCoolent;
    public bool StationFuel;
    public bool StationThermometer;
}
