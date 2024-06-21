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
    public GameObject Tab;


    private int keyTabCode = 0;
    private bool inventory_ck;
    private int TotalSell=0;
    private int Amount = 0;
    //노드
    public GameObject[] nodes;
    public Text[] nodesCount;
    public int[] count;
    public int[] SellCount;
    public int[] salePrice;


    private Transform inventoryTransform;

    void Start()
    {
        inside.SetActive(false);
        poi.SetActive(false);
        inventory.SetActive(false);
        Tab.SetActive(false);
        money.SetActive(false);


        inventoryTransform = inventory.transform;

        for (int i = 0; i < nodesCount.Length; i++)
        {
            nodesCount[i].text = "0";
            count[i] = 0;
            SellCount[i] = 0;
        }
    }

    void Update()
    {
        UpdateInsideActive();
        UpdatePoiActive();
        UpdateInventoryActive();
        UpdateInventoryTabActive();
        UpdateMoneyActive();


        //노드 관련 함수
        nodeCountUpdate();
        Sell();
        Die();
    }

    public void nodeCountUpdate()
    {

        for (int i = 0; i < nodesCount.Length; i++)
        {
            if (nodes[i].name + "(Clone)" == GameValue.NodeName)
            {
                count[i]++;
                SellCount[i] = count[i];
                nodesCount[i].text = count[i].ToString();
                GameValue.GetNode("");
            }
        }
    }

    private void Sell()
    {
        if(GameValue.lived)
        {
            money.SetActive(true);
            GameValue.setMoney();
            for (int i = 0; i < nodesCount.Length; i++)
            {
                Amount = SellCount[i] * salePrice[i];
                TotalSell += Amount;
            }
            GameValue.GetMomey(TotalSell);
            GameValue.lived = false;

            for (int i = 0; i < nodesCount.Length; i++)
            {
                nodesCount[i].text = "0";
                SellCount[i] = 0;
                count[i] = 0;
            }
        }
    }

    private void Die()
    {
        if(PlayerController.Hp==0)
        {
            for (int i = 0; i < nodesCount.Length; i++)
            {
                nodesCount[i].text = "0";
            }
        }     
    }

    void UpdateMoneyActive()
    {
        if (GameValue.lived)
        {
            money.SetActive(true);
            inventory.SetActive(false);
            Tab.SetActive(false);
            GameValue.inside = true;
        }
    }

    void UpdateInsideActive()
    {
        if(GameValue.insideUser<GameValue.MaxUser)
        {
            inside.SetActive(PlayerController.insideActive && !PlayerController.PreViewCam);   

        }
    }

    void UpdatePoiActive()
    {
        bool isPoiActive = PlayerController.Poi;
        poi.SetActive(isPoiActive);

        if (isPoiActive)
        {
            inventory.SetActive(true);
            Tab.SetActive(true);
            money.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void Bt_poiExt()
    {
        poi.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerController.Poi = false;
        inventory.SetActive(false);
        Tab.SetActive(false);
    }

    void UpdateInventoryActive()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory_ck = !inventory_ck;
            inventory.SetActive(inventory_ck);
            Tab.SetActive(inventory_ck);
            //money.SetActive(!inventory_ck);
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