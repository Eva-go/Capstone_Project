using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFuel : MonoBehaviour
{
    private PoiController poiController;
    private PlayerController playerController;
    public ScriptableObject_Item SelectableFuel;

    public void RegisterStationController(PoiController station)
    {
        poiController = station;
    }
    public void RegisterPlayerController(PlayerController player)
    {
        playerController = player;
    }
    public void Select_Fuel()
    {
        poiController.SelectedFuel = SelectableFuel;

        if (poiController.Inv_Fuel.ItemType != SelectableFuel)
        {
            playerController.PlayerInventory.AddItem(new Item
            {
                ItemType = poiController.Inv_Fuel.ItemType,
                Count = poiController.Inv_Fuel.Count
            });
            poiController.EmptyItem(0, 2);
        }

        int ItemRequireCount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) ItemRequireCount *= 10;
        if (Input.GetKey(KeyCode.LeftControl)) ItemRequireCount *= 5;

        int ItemRequireCounts = 1;
        int ItemInputCounts = 1;

        ScriptableObject_Recipe SelectedRecipe = playerController.UISelectedPOIController.SelectedRecipe;

        for (int i = 0; i < SelectedRecipe.InputCount; i++)
        {
            if (poiController.Inv_Fuel != null)
            {
                ItemRequireCounts =
                    poiController.
                    Inv_Fuel.ItemType.MaxCount -
                    poiController.
                    Inv_Fuel.Count;
            }
            else
            {
                ItemRequireCounts = SelectableFuel.MaxCount;
            }

            if (!Input.GetKey(KeyCode.LeftAlt) && ItemRequireCounts > ItemRequireCount) ItemRequireCounts = ItemRequireCount;
            if (playerController.PlayerInventory.RemoveItem(new Item
            {
                ItemType = SelectableFuel,
                Count = ItemRequireCounts
            }))
            {
                ItemInputCounts = ItemRequireCounts;
            }
            else
                ItemInputCounts = playerController.PlayerInventory.ClearItem(SelectableFuel);
            poiController.Item_Input(0, 2, SelectableFuel, ItemInputCounts);
        }
        playerController.InvokeInventoryChanged();
        //playerController.PlayerInventory.RemoveItem(new Item { ItemType = SelectableFuel, Count = 1 });
    }
}
