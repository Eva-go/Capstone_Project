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

    public ScriptableObject_Station[] SelectableRecipies;

    public Item[] Inv_Input;
    public Item[] Inv_Output;
    public Item Inv_Fuel;
    public Item Inv_Coolent;

    private float Heating;

    private float StationTemperture;
    private float StationProgress;

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

        propertyBlock = new MaterialPropertyBlock();
    }

    public void UpdateMatStation()
    {
        for (int i = 0; i < StationBases.Length; i++)
        {
            StationBases[i].GetComponent<MeshRenderer>().material = StationBaseMat.ItemLUM;
        }
        for (int i = 0; i < StationFixes.Length; i++)
        {
            StationFixes[i].GetComponent<MeshRenderer>().material = StationFixMat.ItemLUM;
        }
        for (int i = 0; i < StationAuxes.Length; i++)
        {
            StationAuxes[i].GetComponent<MeshRenderer>().material = StationAuxMat.ItemLUM;
        }

        for (int i = 0; i< StationConMat.Length; i++)
        {
            for (int j = 0; j < StationCons[i].Count.Length; j++)
            {
                StationCons[i].Count[j].GetComponent<MeshRenderer>().material = StationConMat[i].ItemLUM;
            }
        }
    }

    public void UpdateMatInventory(GameObject MatObj, Item InvItem)
    {
        float InvAmount;
        Texture InvLU;

        InvAmount = InvItem.Count / InvItem.ItemType.MaxCount;
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

    public void CheckRecipe(ScriptableObject_Station SelectedRecipe)
    {
        if (StationTemperture >= SelectedRecipe.Temperture && Inv_Coolent.Count >= SelectedRecipe.Coolent)
        {
            if (Inv_Input[0].ItemType == SelectedRecipe.Input001
                && Inv_Input[1].ItemType == SelectedRecipe.Input002
                && Inv_Input[2].ItemType == SelectedRecipe.Input003)
            {

            }

            if (StationProgress > SelectedRecipe.ProgressTime)
            {
                ProcessRecipe(SelectedRecipe);
            }
        }
    }

    public void ProcessRecipe(ScriptableObject_Station SelectedRecipe)
    {
        for (int i = 0; i < SelectedRecipe.InputCount; i++)
        {
            Inv_Input[i].SubtractItem(1);
        }
        switch (SelectedRecipe.OutputCount)
        {
            case 1:
                Inv_Output[0].AddItem(SelectedRecipe.Output001, 1);
                break;
            case 2:
                Inv_Output[0].AddItem(SelectedRecipe.Output001, 1);
                Inv_Output[1].AddItem(SelectedRecipe.Output002, 1);
                break;
            case 3:
                Inv_Output[0].AddItem(SelectedRecipe.Output001, 1);
                Inv_Output[1].AddItem(SelectedRecipe.Output002, 1);
                Inv_Output[2].AddItem(SelectedRecipe.Output003, 1);
                break;
        }
    }
    
    public void HeatingManage(ScriptableObject_Station SelectedRecipe)
    {
        if (StationTemperture < SelectedRecipe.Temperture)
        {
            if (Heating < 1)
            {
                if (Inv_Fuel.Count > 0)
                {
                    Heating++;
                    Inv_Fuel.SubtractItem(1);
                    for(int i = 0; i < FuelSlot.Length; i++)
                    {
                        UpdateMatFill(FuelSlot[i], Inv_Fuel.Count / Inv_Fuel.ItemType.MaxCount);
                    }
                }
            }
            if (Heating > 0)
            {

            }
            for(int i = 0; i < Obj_Temperture.Length; i++)
            {
                UpdateMatFill(Obj_Temperture[i], StationTemperture / 250);
            }
        }
        if (StationTemperture > 25)
        {

            for (int i = 0; i < Obj_Temperture.Length; i++)
            {
                UpdateMatFill(Obj_Temperture[i], StationTemperture / 250);
            }
        }
    }





    [PunRPC]
    public void ReceiveData(int nodeItemCount, string nodeName, string playerNickName, int i)
    {
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
