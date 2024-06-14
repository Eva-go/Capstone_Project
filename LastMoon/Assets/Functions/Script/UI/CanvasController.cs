using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject poi;
    public GameObject inside;
    public GameObject inventory;
    public GameObject money;

    private int keyTabCode = 0;
    private bool inventory_ck;

    private Transform inventoryTransform;

    void Start()
    {
        inside.SetActive(false);
        poi.SetActive(false);
        inventory.SetActive(false);
        money.SetActive(true);

        inventoryTransform = inventory.transform;
    }

    void Update()
    {
        UpdateInsideActive();
        UpdatePoiActive();
        UpdateInventoryActive();
        UpdateInventoryTabActive();
    }

    void UpdateInsideActive()
    {
        inside.SetActive(PlayerController.insideActive && !PlayerController.PreViewCam);
    }

    void UpdatePoiActive()
    {
        bool isPoiActive = PlayerController.Poi;
        poi.SetActive(isPoiActive);

        if (isPoiActive)
        {
            inventory.SetActive(true);
            money.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            money.SetActive(true);
        }
    }

    public void Bt_poiExt()
    {
        poi.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerController.Poi = false;
        inventory.SetActive(false);
    }

    void UpdateInventoryActive()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory_ck = !inventory_ck;
            inventory.SetActive(inventory_ck);
            money.SetActive(!inventory_ck);
        }
    }

    void UpdateInventoryTabActive()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            keyTabCode = (keyTabCode + 1) % 3;
            bool isCraftManualActive = CraftMaunal.isActivated && !CraftMaunal.isPreViewActivated;

            SetInventoryTabActive(keyTabCode);

            SetTabActive(keyTabCode);
        }
    }

    void SetInventoryTabActive(int index)
    {
        for (int i = 1; i <= 3; i++)
        {
            inventoryTransform.GetChild(i).gameObject.SetActive(i == index + 1);
        }
    }

    void SetTabActive(int index)
    {
        Transform tabTransform = GameObject.Find("Tab").transform;
        for (int i = 0; i <= 2; i++)
        {
            tabTransform.GetChild(i).gameObject.SetActive(i == index);
        }
    }
}