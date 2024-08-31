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

    //--- 건축 재질 메테리얼 오브젝트-------------------

    public GameObject[] Obj_Bases;
    public GameObject[] Obj_Auxes;
    public GameObject[] Obj_Fixes;
    public ObjArray[] Obj_Cons;

    public Inventory StationConstInv = new Inventory { };

    public ScriptableObject_Item StationBaseMat;
    public ScriptableObject_Item StationAuxMat;
    public ScriptableObject_Item StationFixMat;
    public ScriptableObject_Item[] StationConMat;

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
                        if (StationType == 0 && stationController.Constructed) stationController.Constructed = false;
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
                        if (StationType == 0 && !stationController.Constructed) stationController.Constructed = true;
                    }
                }
                else
                {
                    if (StationType == 0 && stationController.Constructed) stationController.Constructed = false;
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

    public void UpdateMatStation()
    {
        UpdateObjArrMat(Obj_Bases, StationBaseMat);
        UpdateObjArrMat(Obj_Fixes, StationFixMat);
        UpdateObjArrMat(Obj_Auxes, StationAuxMat);
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





    /*
    //--- 아이템 슬롯 메테리얼 오브젝트
    public ObjArray[] Obj_InputSlot;
    public ObjArray[] Obj_OutputSlot;

    public int[] Obj_SlotType;

    public GameObject[] Obj_FuelSlot;
    public GameObject[] Obj_FuelWick;

    public GameObject[] Obj_Coolent;
    public GameObject[] Obj_Temperture;
    public bool[] Obj_Additional;


    public void UpdateMatInventory_Solid(GameObject MatObj, Item InvItem)
    {
        float InvAmount;
        InvAmount = (float)InvItem.Count / (float)InvItem.ItemType.MaxCount;

        MatObj.GetComponent<MeshRenderer>().material = InvItem.ItemType.ItemLUM;
        MatObj.GetComponent<Transform>().localScale = new Vector3(InvAmount, InvAmount, InvAmount);
    }
    public void UpdateMatInventory_Liquid(GameObject MatObj, Item InvItem)
    {
        float InvAmount;
        Texture InvLU;

        InvAmount = (float)InvItem.Count / (float)InvItem.ItemType.MaxCount;
        InvLU = InvItem.ItemType.ItemLU;

        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", InvAmount);
        propertyBlock.SetTexture("_Look_Up_Texture", InvLU);
        MatObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
    public void UpdateMatFill(GameObject MatObj, float FillAmount)
    {
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Fill", FillAmount);
        MatObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
     */
}
