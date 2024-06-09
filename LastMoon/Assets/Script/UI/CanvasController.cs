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


    private int KeyTabCode = 0;
    private bool inventory_ck;


    void Start()
    {
        inside.SetActive(false);
        poi.SetActive(false);
        inventory.SetActive(false);
        money.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        InsideActive();
        poiActive();
        inventoryActive();
        inventoryTabActive();
    }

        public void InsideActive()
    {
        inside.SetActive(PlayerController.insideActive);
        if (PlayerController.PreViewCam)
        {
            inside.SetActive(false);
        }
    }

    public void poiActive()
    {
        poi.SetActive(PlayerController.Poi);
       
        if (PlayerController.Poi)
        {
            inventory.SetActive(true);
            money.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
        }
        money.SetActive(true);
    }
    public void Bt_poiExt()
    {
        poi.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerController.Poi = false;
        inventory.SetActive(false);
    }


    public void inventoryActive()
    {
        if(Input.GetKeyDown(KeyCode.I)&&!inventory_ck)
        {
            inventory.SetActive(true);
            inventory_ck = true;
            money.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.I) && inventory_ck)
        {
            inventory.SetActive(false);
            inventory_ck = false;
            money.SetActive(true);
        }
            
    }
    public void inventoryTabActive()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            KeyTabCode++;
            if (!CraftMaunal.isPreViewActivated)
            {
                switch (KeyTabCode % 3)
                {
                    case 0:
                        GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(true);
                        GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(false);
                        GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(false);
                        if (CraftMaunal.isActivated)
                        {
                            GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(true);
                            GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(false);
                            GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(false);
                        }
                        break;
                    case 1:
                        GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(false);
                        GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(true);
                        GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(false);
                        if (CraftMaunal.isActivated)
                        {
                            GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(false);
                            GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(true);
                            GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(false);
                        GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(false);
                        GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(true);
                        if (CraftMaunal.isActivated)
                        {
                            GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(false);
                            GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(false);
                            GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(true);
                        }
                        break;
                }
            }

        }
    }
}
