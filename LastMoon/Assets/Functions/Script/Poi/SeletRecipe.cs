using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeletRecipe : MonoBehaviour
{
    private PoiController poiController;
    public ScriptableObject_Station SelectableRecipe;

    public void RegisterStationController(PoiController station)
    {
        poiController = station;
    }
    public void SelectRecipe()
    {
        poiController.SelectedRecipe = SelectableRecipe;
    }
}
