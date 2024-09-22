using System.Collections;
using UnityEngine;
using Photon.Pun;


public class StationMatController : MonoBehaviour
{
    private MaterialPropertyBlock propertyBlock;
    public PoiController stationController;

    public int StationType; // 0 - 기본, 1 - 파이프

    private bool isTick;
    private int tick;

    //---건설&파괴 애니메이션 오브젝트------------------

    public GameObject StationConstructionMesh;
    public GameObject StationConstructionParts;

    public int ConstructionProgress;
    public int Health = 30;
    public int Heal_Cooldown = 0;
    public bool Constructed = false;

    //--- 건축 재질 메테리얼 오브젝트-------------------

    public GameObject[] Obj_Bases;
    public GameObject[] Obj_Auxes;
    public GameObject[] Obj_Fixes;
    public ObjArray[] Obj_Cons;

    public GameObject[] Obj_AuxFix;

    public GameObject[] Obj_AuxLUT;

    public Inventory StationConstInv = new Inventory { };

    public ScriptableObject_Item StationBaseMat;
    public ScriptableObject_Item StationAuxMat;
    public ScriptableObject_Item StationFixMat;
    public ScriptableObject_Item[] StationConMat;

    //--- 아이템 슬롯 메테리얼 오브젝트-----------------

    public ObjArray[] Obj_Slot_Input;
    public ObjArray[] Obj_Slot_Output;
    public int[] Obj_SlotType;

    public GameObject[] Obj_Slot_Fuel;
    public GameObject[] Obj_Slot_FuelWick;
    public GameObject[] Obj_Slot_Coolent;
    public GameObject[] Obj_Slot_Temperture;
    public bool[] Obj_Additional;

    //--------------------------------------------------

    void Start()
    {
        ConstructionProgress = 0;
        Health = 30;
        Heal_Cooldown = 0;
    }
    private void Update()
    {
        tick_ck();
    }
    public void tick_ck()
    {
        if (!isTick)
        {
            isTick = true;
            TickTimer.OnTick += TimeTickSystem_OnTick;
        }
    }
    private void OnDestroy()
    {
        TickTimer.OnTick -= TimeTickSystem_OnTick;
    }
    private void TimeTickSystem_OnTick(object sender, TickTimer.OnTickEventArgs e)
    {
        if (isTick)
        {
            tick = e.tick % 1;
            if (tick >= 0)
            {
                if (Health > 0)
                {
                    if (ConstructionProgress < 100)
                    {
                        if (StationType == 0 && (stationController.Constructed || Constructed))
                        {
                            Constructed = false;
                            stationController.Constructed = false;
                        }
                        if (StationConstructionMesh != null) ConstructionAnimation();
                    }
                    else
                    {
                        Heal_Cooldown++;
                        if (Heal_Cooldown >= 20) 
                        {
                            if (Health < 30) Health++;
                            Heal_Cooldown = 0;
                        }
                        if (StationConstructionMesh != null && StationConstructionMesh.activeSelf)
                        {
                            StationConstructionParts.SetActive(true);
                            StationConstructionMesh.SetActive(false);
                        }
                        if (StationType == 0 && (!stationController.Constructed || !Constructed))
                        {
                            Constructed = true;
                            stationController.Constructed = true;
                        }
                    }
                }
                else
                {
                    if (StationType == 0 && (stationController.Constructed || Constructed))
                    {
                        Constructed = false;
                        stationController.Constructed = false;
                    }
                    DestroyAnimation();
                    if (ConstructionProgress < 0)
                    {
                        Destroy(gameObject);
                    }
                }
                isTick = false;
            }
        }
    }
    //--------------------------------------------------
    public void ConstructionAnimation()
    {
        if (!StationConstructionMesh.activeSelf)
        {
            StationConstructionParts.SetActive(false);
            StationConstructionMesh.SetActive(true);
        }
        ConstructionProgress += 2;
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Construction_Progress", ConstructionProgress / 100f);
        propertyBlock.SetInt("_IsConstuction", 1);
        StationConstructionMesh.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
    public void DestroyAnimation()
    {
        if (!StationConstructionMesh.activeSelf)
        {
            StationConstructionParts.SetActive(false);
            StationConstructionMesh.SetActive(true);
        }
        ConstructionProgress -= 3;
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Construction_Progress", ConstructionProgress / 100f);
        propertyBlock.SetInt("_IsConstuction", 0);
        StationConstructionMesh.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
    //--------------------------------------------------
    public void UpdateMatStation()
    {
        UpdateObjArrMat(Obj_Bases, StationBaseMat);
        UpdateObjArrMat(Obj_Fixes, StationFixMat);
        UpdateObjArrMat(Obj_Auxes, StationAuxMat);
        UpdateObjArrMatLUT(Obj_AuxLUT, StationAuxMat);
        UpdateAuxFixMat();
        for (int i = 0; i < Obj_Cons.Length; i++)
        {
            UpdateObjArrMat(Obj_Cons[i].Count, StationConMat[i]);
        }
    }
    public void UpdateObjArrMat(GameObject[] objects, ScriptableObject_Item item)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = item.ItemLUM;
        }
    }
    public void UpdateObjArrMatLUT(GameObject[] objects, ScriptableObject_Item item)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = item.ItemLUT;
        }
    }
    public void UpdateAuxFixMat()
    {
        for (int i = 0; i < Obj_AuxFix.Length; i++)
        {
            Material[] materials = Obj_AuxFix[i].GetComponent<MeshRenderer>().materials;
            materials[0] = StationAuxMat.ItemLUM;
            materials[1] = StationFixMat.ItemLUM;
            Obj_AuxFix[i].GetComponent<MeshRenderer>().materials = materials;
        }
    }
    //--------------------------------------------------
    public void UpdateMatInventory_Solid(GameObject[] objects, Item InvItem)
    {
        float InvAmount;
        InvAmount = (float)InvItem.Count / (float)InvItem.ItemType.MaxCount;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = InvItem.ItemType.ItemLUM;
            objects[i].GetComponent<Transform>().localScale = new Vector3(InvAmount, InvAmount, InvAmount);
        }
    }
    public void UpdateMatInventory_Liquid(GameObject[] objects, Item InvItem)
    {
        float InvAmount;
        Texture InvLU;

        InvAmount = (float)InvItem.Count / (float)InvItem.ItemType.MaxCount;
        InvLU = InvItem.ItemType.ItemLU;

        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", InvAmount);
        propertyBlock.SetTexture("_Look_Up_Texture", InvLU);

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        }
    }
    public void UpdateMatFill(GameObject[] objects, float FillAmount)
    {
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", FillAmount);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        }
    }
    public void UpdateFuelInventory()
    {
        Item Inv_Fuel = stationController.Inv_Fuel;
        float InvAmount;
        Texture InvLU;

        InvAmount = (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount;
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", InvAmount);
        
        for (int i = 0; i < Obj_Slot_FuelWick.Length; i++)
        {
            Obj_Slot_FuelWick[i].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        }

        InvLU = Inv_Fuel.ItemType.ItemLU;
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", InvAmount);
        propertyBlock.SetTexture("_Look_Up_Texture", InvLU);

        for (int i = 0; i < Obj_Slot_Fuel.Length; i++)
        {
            Obj_Slot_Fuel[i].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        }
    }
    public void UpdateTemperture()
    {
        UpdateMatFill(Obj_Slot_Temperture, stationController.StationTemperture / 250);
    }
    //--------------------------------------------------
    public void UpdateInventories()
    {
        for (int i = 0; i < Obj_Slot_Input.Length; i++)
        {
            if (Obj_Slot_Input[i].Count != null)
            {
                if (Obj_SlotType[i] == 1) UpdateMatInventory_Liquid(Obj_Slot_Input[i].Count, stationController.Inv_Input[i]);
                else if (Obj_SlotType[i] == 2) UpdateMatInventory_Solid(Obj_Slot_Input[i].Count, stationController.Inv_Input[i]);
            }
        }
        for (int i = 0; i < Obj_Slot_Output.Length; i++)
        {
            if (Obj_Slot_Output[i].Count != null)
            {
                if (Obj_SlotType[i + 3] == 1) UpdateMatInventory_Liquid(Obj_Slot_Output[i].Count, stationController.Inv_Output[i]);
                else if (Obj_SlotType[i + 3] == 2) UpdateMatInventory_Solid(Obj_Slot_Output[i].Count, stationController.Inv_Output[i]);
            }
        }
    }
}
