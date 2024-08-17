using System.Collections;
using UnityEngine;
using Photon.Pun;


[System.Serializable]
public struct ItemData
{
    public string[] nodeName;
    public int[] nodeItemCount;
    public string[] mixName;
    public int[] mixItemCount;
}

[System.Serializable]
public class ObjArray
{
    public GameObject[] Count;
}

public class PoiController : MonoBehaviour
{
    public PhotonView pv;
    public ItemData itemData;
    public string poiName;
    public int SetnodeCount;
    public Animator animator;
    private string playerName = " ";
    private bool processing = false;

    public GameObject[] StationBases;
    public GameObject[] StationFixes;
    public GameObject[] StationAuxes;
    public ObjArray[] StationCons;

    public ScriptableObject_Item Debug_Filler;

    public ScriptableObject_Item StationBaseMat;
    public ScriptableObject_Item StationAuxMat;
    public ScriptableObject_Item StationFixMat;
    public ScriptableObject_Item[] StationConMat;

    private MaterialPropertyBlock propertyBlock;

    public ObjArray[] InputSlot;
    public ObjArray[] OutputSlot;
    public GameObject[] FuelSlot;
    public GameObject[] FuelWick;

    public GameObject[] Obj_Coolent;
    public GameObject[] Obj_Temperture;

    public ScriptableObject_Station[] SelectableRecipes;
    public ScriptableObject_Station SelectedRecipe;

    public Item[] Inv_Input = new Item[3];
    public Item[] Inv_Output = new Item[3];
    public Item Inv_Fuel;
    public Item Inv_Coolent;

    private float Heating;

    private float StationTemperture;
    private float StationProgress;

    //tick  관련 변수
    public int tick =0 ;
    public int tickMax = 0;
    public bool isConstructing;

    //레시피 관련 변수
    private int slotNumber; // 현재 슬롯 번호


    //컨베이어 벨트 관련 변수
    public GameObject testObject;
    public Transform testObjectTransform;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        processing = false;
        SetnodeCount = 0;
        itemData.nodeItemCount = new int[6];
        itemData.mixItemCount = new int[6];
        itemData.nodeName = new string[] { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap" };
        itemData.mixName = new string[] { "clayware", "porcelain", "quartz_glass", "sodaLime_glass", "iron", "steel" };
        poiName = gameObject.name;
        for (int i = 0; i < itemData.nodeItemCount.Length; i++)
        {
            itemData.nodeItemCount[i] = 0;
            itemData.mixItemCount[i] = 0;
        }

        StationProgress = 0;
        propertyBlock = new MaterialPropertyBlock();
    }


    private void Update()
    {
        tick_ck(100);
    }

    public void tick_ck(int ticksToConstruct)
    {
        
        if (!isConstructing)
        {
            tickMax = ticksToConstruct;
            isConstructing = true;
            TickTimer.OnTick += TimeTickSystem_OnTick;
        }

    }

    private void TimeTickSystem_OnTick(object sender, TickTimer.OnTickEventArgs e)
    {
        if (isConstructing)
        {
            tick = e.tick% tickMax;
            Debug.Log("E tIck" + tick+" : "+e.tick);
            if (tick>=tickMax-1)
            {
                //CheckRecipe();
                //HeatingManage();
                if (testObject != null && testObjectTransform != null)
                {
                    Instantiate(testObject, testObjectTransform.position, Quaternion.identity);
                }
                isConstructing = false;
            }
            else
            {
              //매 틱마다 해야한다면 이곳
            }
           
        }
    }



    //레시피
    public void SlotClick(int _slotNumber)
    {
        onClickStart(_slotNumber); // Call the start method for preview
    }

    public void onClickStart(int _slotNumber)
    {
        for (int i = 0; i < SelectableRecipes.Length; i++)
        {
            if (i == _slotNumber)
            {
                SelectedRecipe = SelectableRecipes[i];
            }
        }
    }


    public Item AddItem(GameObject[] objects, Item item, ScriptableObject_Item Type, int Count)
    {
        if (item == null)
        {
            item = new Item { ItemType = Type, Count = Count };
            for (int i = 0; i < objects.Length; i++)
            {
                UpdateMatInventory(objects[i], item);
            }
        }
        else
        {
            int Overflow = item.OverrideItem(Type, Count);
            if (Overflow == -1)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    UpdateMatInventory(objects[i], item);
                }
            }
            else
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    UpdateMatFill(objects[i], (float)item.Count / (float)item.ItemType.MaxCount);
                }
            }
        }
        return item;
    }

    public Item SubtractItem(GameObject[] objects, Item item, int Count)
    {
        item.SubtractItem(Count);
        for (int i = 0; i < objects.Length; i++)
        {
            UpdateMatFill(objects[i], (float)item.Count / (float)item.ItemType.MaxCount);
        }
        return item;
    }


    public void UpdateMatStation()
    {
        UpdateObjMat(StationBases, StationBaseMat);
        UpdateObjMat(StationFixes, StationFixMat);
        UpdateObjMat(StationAuxes, StationAuxMat);
        for (int i = 0; i< StationConMat.Length; i++)
        {
            UpdateObjMat(StationCons[i].Count, StationConMat[i]);
        }
    }
    public void UpdateObjMat(GameObject[] objects, ScriptableObject_Item item)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = item.ItemLUM;
        }
    }
    public void UpdateMatInventory(GameObject MatObj, Item InvItem)
    {
        float InvAmount;
        Texture InvLU;

        InvAmount = (float)InvItem.Count / (float)InvItem.ItemType.MaxCount;
        InvLU = InvItem.ItemType.ItemLU;

        propertyBlock.SetFloat("_Fill", InvAmount);
        propertyBlock.SetTexture("_Look_Up_Texture", InvLU);
        MatObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
    public void UpdateMatFill(GameObject MatObj, float FillAmount)
    {
        propertyBlock.SetFloat("_Fill", FillAmount);
        MatObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }


    public void CheckRecipe()
    {
        Debug.Log("Tick executed at CheckRecipe()");
        //if (StationTemperture >= SelectedRecipe.Temperture && Inv_Coolent.Count >= SelectedRecipe.Coolent)
        //{
        //    if (Inv_Input[0].ItemType == SelectedRecipe.Input001
        //        && Inv_Input[1].ItemType == SelectedRecipe.Input002
        //        && Inv_Input[2].ItemType == SelectedRecipe.Input003)
        //    {
        //        StationProgress++;
        //    }
        //    if (StationProgress > SelectedRecipe.ProgressTime)
        //    {
        //        StationProgress = 0;
        //        for (int i = 0; i < SelectedRecipe.InputCount; i++)
        //        {
        //            Inv_Input[i] = SubtractItem(InputSlot[i].Count, Inv_Input[i], 1);
        //        }
        //        switch (SelectedRecipe.OutputCount)
        //        {
        //            case 1:
        //                Inv_Output[0] = AddItem(OutputSlot[0].Count, Inv_Output[0], SelectedRecipe.Output001, 1);
        //                break;
        //            case 2:
        //                Inv_Output[0] = AddItem(OutputSlot[0].Count, Inv_Output[0], SelectedRecipe.Output001, 1);
        //                Inv_Output[1] = AddItem(OutputSlot[1].Count, Inv_Output[1], SelectedRecipe.Output002, 1);
        //                break;
        //            case 3:
        //                Inv_Output[0] = AddItem(OutputSlot[0].Count, Inv_Output[0], SelectedRecipe.Output001, 1);
        //                Inv_Output[1] = AddItem(OutputSlot[1].Count, Inv_Output[1], SelectedRecipe.Output002, 1);
        //                Inv_Output[2] = AddItem(OutputSlot[2].Count, Inv_Output[2], SelectedRecipe.Output003, 1);
        //                break;
        //        }
        //    }
        //}
    }
    
    public void HeatingManage()
    {
        Debug.Log("Tick executed at HeatingManage()");
        //if (StationTemperture < SelectedRecipe.Temperture)
        //{
        //    if (Inv_Fuel.Count > 0)
        //    {
        //        StationTemperture += Inv_Fuel.ItemType.Heating;
        //        Inv_Fuel.SubtractItem(1);
        //        for (int i = 0; i < FuelSlot.Length; i++)
        //        {
        //            UpdateMatFill(FuelSlot[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
        //            UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
        //        }
        //    }
        //}
        //if (StationTemperture > 25)
        //{
        //    if (StationTemperture > 100 && Inv_Coolent.Count > 0)
        //    {
        //        StationTemperture -= 2;
        //        Inv_Coolent.SubtractItem(1);
        //        for (int i = 0; i < Obj_Coolent.Length; i++)
        //        {
        //            UpdateMatFill(Obj_Coolent[i], (float)Inv_Coolent.Count / (float)Inv_Coolent.ItemType.MaxCount);
        //        }
        //    }
        //    else
        //    {
        //        StationTemperture--;
        //    }
        //}
        //for (int i = 0; i < Obj_Temperture.Length; i++)
        //{
        //    UpdateMatFill(Obj_Temperture[i], StationTemperture / 250);
        //}
    }



    [PunRPC]
    public void ReceiveData(int nodeItemCount, string nodeName, string playerNickName, int i)
    {
        Inv_Input[0] = AddItem(InputSlot[0].Count, Inv_Input[0], Debug_Filler, 1);

        if (itemData.nodeName[i].Equals(nodeName) && nodeItemCount >= 0)
        {
            if (playerName == " ")
            {
                SetnodeCount = nodeItemCount;
                itemData.nodeItemCount[i]++;
                playerName = playerNickName;
            }
            else if (playerName == playerNickName)
            {
                itemData.nodeItemCount[i]++;
            }

            if (!processing)
            {
                StartCoroutine(ProcessItems(i));
            }
        }
    }

    private IEnumerator ProcessItems(int i)
    {
        processing = true;
        animator.SetBool("isActvie", true);
        while (itemData.nodeItemCount[i] > 0)
        {
            itemData.nodeItemCount[i]--;
            yield return new WaitForSeconds(5f);
            itemData.mixItemCount[i]++;
        }
        itemData.nodeItemCount[i] = 0;
        processing = false;
        animator.SetBool("isActvie", false);
    }

    [PunRPC]
    public void UpdatePlayerNodeItem(int index, int newCount)
    {
        PlayerController localPlayer = LocalPlayerManger.Instance.GetLocalPlayer();
        if (localPlayer != null)
        {
            localPlayer.UpdateNodeItem(index, newCount);
        }
    }

    [PunRPC]
    public int GetMixItem(int i)
    {
        int mixoldItem = itemData.mixItemCount[i];
        itemData.mixItemCount[i] = 0;
        return mixoldItem;
    }
}
