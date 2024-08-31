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

    public float ConstructionProgress;

    public GameObject StationConstructionMesh;
    public GameObject StationConstructionParts;

    public bool[] ObjectlessSlot = new bool[9];
    public int[] InputSlotType = new int[3];
    public int[] OutputSlotType = new int[3];

    public GameObject[] StationBases;
    public GameObject[] StationFixes;
    public GameObject[] StationAuxes;
    public ObjArray[] StationCons;

    public Inventory StationConstInv = new Inventory { };

    public ScriptableObject_Item Debug_Filler;
    public ScriptableObject_Item Debug_Fuel;
    public ScriptableObject_Item Debug_Coolent;

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

    public ScriptableObject_Recipe[] SelectableRecipes;
    public ScriptableObject_Recipe SelectedRecipe;

    public Item[] Inv_Input = new Item[3];
    public Item[] Inv_Output = new Item[3];
    public Item Inv_Fuel;
    public Item Inv_Coolent;

    public float StationTemperture = 25;
    public float StationProgress;

    public AudioSource sfx_Station_Start, sfx_Station_Activation;
    public ParticleSystem Particle_Activation;

    private int CoolentDrain;

    private bool Activation;
    private bool Refilling;
    private bool Item_Filled;
    private bool Heating;

    private bool ActivationType_Heating;

    public float MaxHealth = 10;
    public float ProcessEfficiency = 1;
    public float TempertureLimit = 100;

    //tick  관련 변수
    public int tick;
    public int tickMax;
    public bool isConstructing;
    public bool stop;



    //레시피 관련 변수
    private int slotNumber; // 현재 슬롯 번호

    //파괴 변수
    public int hp = 30;


    //출력 변수
    public GameObject item;
    public Transform OutputTransform;

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
        hp = 30;
        StationProgress = 0;
        Inv_Fuel = new Item { ItemType = new ScriptableObject_Item { }, Count = 0 };
        Inv_Coolent = new Item { ItemType = new ScriptableObject_Item { }, Count = 0 };
        //test_ck = false;

        isConstructing = false;

        Activation = false;
        Heating = false;
        Refilling = true;
        CoolentDrain = 0;
    }

    //TODO 업데이트
    private void Update()
    {
        ActivationEffect();
        tick_ck(1);
        GiveItem(1);

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
        if (isConstructing && !stop)
        {
            tick = e.tick % tickMax;
            if (tick >= tickMax - 1)
            {
                if (hp > 0)
                {
                    if (ConstructionProgress < 100)
                    {
                        if (StationConstructionMesh != null) ConstructionAnimation();
                    }
                    else
                    {
                        if (StationConstructionMesh != null && StationConstructionMesh.activeSelf)
                        {
                            StationConstructionParts.SetActive(true);
                            StationConstructionMesh.SetActive(false);
                        }

                        if (SelectedRecipe != null)
                        {
                            CheckRecipe();
                            HeatingManage();

                            if (!Activation || Refilling)
                            {
                                if (ActivationType_Heating)
                                {
                                    Inv_Fuel = AddItem(Inv_Fuel, Debug_Fuel, 1);
                                    if (!ObjectlessSlot[6])
                                    {
                                        for (int i = 0; i < FuelSlot.Length; i++)
                                        {
                                            if (FuelSlot[i] != null) UpdateMatInventory(FuelSlot[i], Inv_Fuel);
                                            if (FuelWick[i] != null) UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
                                        }
                                    }
                                }
                                if (SelectedRecipe.Coolent > 0)
                                {
                                    Inv_Coolent = AddItem(Inv_Coolent, Debug_Coolent, 1);
                                    if (Obj_Coolent != null)
                                    {
                                        for (int i = 0; i < Obj_Coolent.Length; i++)
                                        {
                                            UpdateMatInventory(Obj_Coolent[i], Inv_Coolent);
                                        }
                                    }
                                }
                            }
                            if ((!ActivationType_Heating || Inv_Fuel.Count >= 75) &&
                                (SelectedRecipe.Coolent <= 0 || Inv_Coolent.Count >= 75))
                                Refilling = false;
                        }
                    }
                }
                else
                {
                    DestroyAnimation();
                    if (ConstructionProgress < 0)
                    {
                        Destroy(gameObject);
                    }
                }
                isConstructing = false;
            }
        }
    }

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

    public void TakeDamage(float Damage)
    {
        hp -= (int)Damage;
        if (hp > 0)
        {
            animator.SetTrigger("isHit");

        }
        else if (hp <= 0)
        {
            if (!StationConstructionMesh.activeSelf)
            {
                StationConstructionParts.SetActive(false);
                StationConstructionMesh.SetActive(true);
            }
        }
    }

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

    public Item AddItem(Item item, ScriptableObject_Item Type, int Count)
    {
        if (item == null)
        {
            item = new Item { ItemType = Type, Count = Count };
        }
        else
        {
            item.OverrideItem(Type, Count);
        }
        return item;
    }
    public Item SubtractItem(Item item, int Count)
    {
        item.SubtractItem(Count);
        return item;
    }


    public void UpdateMatStation()
    {
        UpdateObjMat(StationBases, StationBaseMat);
        UpdateObjMat(StationFixes, StationFixMat);
        UpdateObjMat(StationAuxes, StationAuxMat);
        for (int i = 0; i < StationConMat.Length; i++)
        {
            UpdateObjMat(StationCons[i].Count, StationConMat[i]);
        }
    }
    public void UpdateMatInventories()
    {
        for (int i = 0; i < InputSlot.Length; i++)
        {
            if (InputSlot[i].Count != null)
            {
                for (int j = 0; j < InputSlot[i].Count.Length; j++)
                {
                    if (InputSlotType[i] == 1) UpdateMatInventory_Liquid(InputSlot[i].Count[j], Inv_Input[i]);
                    else if (InputSlotType[i] == 2) UpdateMatInventory_Solid(InputSlot[i].Count[j], Inv_Input[i]);
                }
            }
        }
        for (int i = 0; i < OutputSlot.Length; i++)
        {
            if (OutputSlot[i].Count != null)
            {
                for (int j = 0; j < OutputSlot[i].Count.Length; j++)
                {
                    if (InputSlotType[i] == 1) UpdateMatInventory_Liquid(OutputSlot[i].Count[j], Inv_Output[i]);
                    else if (InputSlotType[i] == 2) UpdateMatInventory_Solid(OutputSlot[i].Count[j], Inv_Output[i]);
                }
            }
        }
    }

    public void UpdateObjMat(GameObject[] objects, ScriptableObject_Item item)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = item.ItemLUM;
        }
    }

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


    public void UpdateMatInventory(GameObject MatObj, Item InvItem)
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




    public void CheckRecipe()
    {
        Debug.Log("Tick executed at CheckRecipe()");
        switch (SelectedRecipe.InputCount)
        {
            case 1:
                if (Inv_Input[0] != null)
                {
                    Item_Filled = (Inv_Input[0].ItemType == SelectedRecipe.Input[0] && Inv_Input[0].Count > 0);
                }
                break;
            case 2:
                if (Inv_Input[0] != null && Inv_Input[1] != null)
                {
                    Item_Filled = (Inv_Input[0].ItemType == SelectedRecipe.Input[0] && Inv_Input[0].Count > 0
                    && Inv_Input[1].ItemType == SelectedRecipe.Input[1] && Inv_Input[1].Count > 0);
                }
                break;
            case 3:
                if (Inv_Input[0] != null && Inv_Input[1] != null && Inv_Input[2] != null)
                {
                    Item_Filled = (Inv_Input[0].ItemType == SelectedRecipe.Input[0] && Inv_Input[0].Count > 0
                    && Inv_Input[1].ItemType == SelectedRecipe.Input[1] && Inv_Input[1].Count > 0
                    && Inv_Input[2].ItemType == SelectedRecipe.Input[2] && Inv_Input[2].Count > 0);
                }
                break;
        }

        Activation = (
            !Refilling &&
            Item_Filled &&
            StationTemperture >= SelectedRecipe.Temperture &&
            Inv_Coolent.Count >= SelectedRecipe.Coolent
            );


        if (CoolentDrain > 0)
        {
            CoolentDrain--;
            Inv_Coolent.SubtractItem(1);
            if (Inv_Coolent.Count <= SelectedRecipe.Coolent) Refilling = true;
            if (!ObjectlessSlot[7])
            {
                for (int i = 0; i < Obj_Coolent.Length; i++)
                {
                    UpdateMatFill(Obj_Coolent[i], (float)Inv_Coolent.Count / (float)Inv_Coolent.ItemType.MaxCount);
                }
            }
        }

        if (Activation)
        {
            StationProgress += ProcessEfficiency;
            if (StationProgress >= SelectedRecipe.ProgressTime)
            {
                StationProgress = 0;
                if (hp < 30) hp++;
                if (SelectedRecipe.Coolent > 0) CoolentDrain = (int)SelectedRecipe.Coolent;
                for (int i = 0; i < SelectedRecipe.InputCount; i++)
                {
                    Inv_Input[i] = SubtractItem(Inv_Input[i], 1);
                    if (i < InputSlot.Length && InputSlot[i].Count != null)
                    {
                        for (int j = 0; j < InputSlot[i].Count.Length; j++)
                        {
                            UpdateMatInventory(InputSlot[i].Count[j], Inv_Input[i]);
                        }
                    }
                }
                for (int i = 0; i < SelectedRecipe.OutputCount; i++)
                {
                    Inv_Output[i] = AddItem(Inv_Output[i], SelectedRecipe.Output[i], 1);
                    if (i < OutputSlot.Length && OutputSlot[i].Count != null)
                    {
                        for (int j = 0; j < OutputSlot[i].Count.Length; j++)
                        {
                            UpdateMatInventory(OutputSlot[i].Count[j], Inv_Output[i]);
                        }
                    }
                }

            }
        }

    }
    public void HeatingManage()
    {
        if (!ActivationType_Heating && SelectedRecipe.Temperture > 0) ActivationType_Heating = true;
        if (ActivationType_Heating && !Refilling)
        {
            if (Item_Filled)
            {
                if (StationTemperture < SelectedRecipe.Temperture + 5)
                {
                    if (Inv_Fuel.Count > 0)
                    {
                        Heating = true;
                        StationTemperture += Inv_Fuel.ItemType.Heating;
                        Inv_Fuel.SubtractItem(1);
                        if (Inv_Fuel.Count <= 0) Refilling = true;
                        if (!ObjectlessSlot[6])
                        {
                            for (int i = 0; i < FuelSlot.Length; i++)
                            {
                                if (FuelSlot[i] != null) UpdateMatInventory(FuelSlot[i], Inv_Fuel);
                                if (FuelWick[i] != null) UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
                            }
                        }
                    }
                    else Heating = false;
                }
            }
            else Heating = false;
            if (StationTemperture > 25)
            {
                StationTemperture--;
            }
            if (!ObjectlessSlot[8])
            {
                for (int i = 0; i < Obj_Temperture.Length; i++)
                {
                    if (Obj_Temperture[i] != null) UpdateMatFill(Obj_Temperture[i], StationTemperture / 250);
                }
            }
        }
    }
    public void ActivationEffect()
    {
        if (!Refilling && (Activation || Heating))
        {
            if (animator != null) animator.SetBool("isActvie", true);
            if (sfx_Station_Activation != null && !sfx_Station_Activation.isPlaying)
            {
                if (sfx_Station_Start != null) sfx_Station_Start.Play();
                sfx_Station_Activation.Play();
            }
            if (Particle_Activation != null && !Particle_Activation.isPlaying)
            {
                Particle_Activation.Play();
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isActvie", false);
            if (sfx_Station_Activation != null && sfx_Station_Activation.isPlaying)
            {
                if (sfx_Station_Start != null) sfx_Station_Start.Stop();
                sfx_Station_Activation.Stop();
            }
            if (Particle_Activation != null && Particle_Activation.isPlaying)
            {
                Particle_Activation.Stop();
            }
        }
    }




    public void InputItems(int ItemRequireCount)
    {
        for (int i = 0; i < SelectedRecipe.InputCount; i++)
        {
            Inv_Input[i] = AddItem(Inv_Input[i], SelectedRecipe.Input[i], ItemRequireCount);
            if (i < InputSlot.Length && InputSlot[i].Count != null)
            {
                for (int j = 0; j < InputSlot[i].Count.Length; j++)
                {
                    UpdateMatInventory(InputSlot[i].Count[j], Inv_Input[i]);
                }
            }
        }
    }
    public void InputItem(int Inv_Slot, int ItemRequireCount)
    {
        Inv_Input[Inv_Slot] = AddItem(Inv_Input[Inv_Slot], SelectedRecipe.Input[Inv_Slot], ItemRequireCount);
        if (Inv_Slot < InputSlot.Length && InputSlot[Inv_Slot].Count != null)
        {
            for (int j = 0; j < InputSlot[Inv_Slot].Count.Length; j++)
            {
                UpdateMatInventory(InputSlot[Inv_Slot].Count[j], Inv_Input[Inv_Slot]);
            }
        }
    }
    public void EmptyItem(int Inv_Slot, int ExtractType)
    {
        switch (ExtractType) // 0 - Input, 1 - Output, 2 - Fuel, 3 - Coolent
        {
            case 0:
                Inv_Input[Inv_Slot].ClearItem();
                if (Inv_Slot < InputSlot.Length && InputSlot[Inv_Slot].Count != null)
                {
                    for (int i = 0; i < InputSlot[Inv_Slot].Count.Length; i++)
                    {
                        UpdateMatInventory(InputSlot[Inv_Slot].Count[i], Inv_Input[Inv_Slot]);
                    }
                }
                break;
            case 1:
                Inv_Output[Inv_Slot].ClearItem();
                if (Inv_Slot < OutputSlot.Length && OutputSlot[Inv_Slot].Count != null)
                {
                    for (int i = 0; i < OutputSlot[Inv_Slot].Count.Length; i++)
                    {
                        UpdateMatInventory(OutputSlot[Inv_Slot].Count[i], Inv_Output[Inv_Slot]);
                    }
                }
                break;
            case 2:
                Inv_Fuel.ClearItem();
                if (FuelSlot != null)
                {
                    for (int i = 0; i < FuelSlot.Length; i++)
                    {
                        UpdateMatInventory(FuelSlot[i], Inv_Fuel);
                        if (FuelWick[i] != null) UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
                    }
                }
                break;
            case 3:
                Inv_Coolent.ClearItem();
                if (Inv_Coolent != null)
                {
                    for (int i = 0; i < Obj_Coolent.Length; i++)
                    {
                        UpdateMatInventory(Obj_Coolent[i], Inv_Coolent);
                    }
                }
                break;
        }
    }


    // 맞는 유형 아이템 받기
    public void TakeItem(Item InputItem)
    {
        int activeSlot = -1;
        if (SelectedRecipe != null)
        {
            for (int i = 0; i < SelectedRecipe.InputCount; i++)
            {
                if (SelectedRecipe.Input[i] == InputItem.ItemType && activeSlot == -1)
                {
                    activeSlot = i;
                }
            }
        }
        if (activeSlot != -1)
        {
            Debug.Log("충돌4");
            Item_Input(activeSlot, 0, InputItem.ItemType, InputItem.Count);
        }
    }


    public void Item_Input(int Inv_Slot, int SlotType, ScriptableObject_Item ItemType, int Count)
    {
        switch (SlotType) // 0 - Input, 1 - Output, 2 - Fuel, 3 - Coolent
        {
            case 0:
                Inv_Input[Inv_Slot] = AddItem(Inv_Input[Inv_Slot], ItemType, Count);
                if (Inv_Slot < InputSlot.Length && InputSlot[Inv_Slot].Count != null)
                {
                    for (int j = 0; j < InputSlot[Inv_Slot].Count.Length; j++)
                    {
                        UpdateMatInventory(InputSlot[Inv_Slot].Count[j], Inv_Input[Inv_Slot]);
                    }
                }
                break;
            case 1:
                Inv_Output[Inv_Slot] = AddItem(Inv_Output[Inv_Slot], ItemType, Count);
                if (Inv_Slot < OutputSlot.Length && OutputSlot[Inv_Slot].Count != null)
                {
                    for (int j = 0; j < OutputSlot[Inv_Slot].Count.Length; j++)
                    {
                        UpdateMatInventory(OutputSlot[Inv_Slot].Count[j], Inv_Output[Inv_Slot]);
                    }
                }
                break;
            case 2:
                Inv_Fuel = AddItem(Inv_Fuel, ItemType, Count);
                if (FuelSlot != null)
                {
                    for (int i = 0; i < FuelSlot.Length; i++)
                    {
                        UpdateMatInventory(FuelSlot[i], Inv_Fuel);
                        if (FuelWick[i] != null) UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
                    }
                }
                break;
            case 3:
                Inv_Coolent = AddItem(Inv_Coolent, ItemType, Count);
                if (Inv_Coolent != null)
                {
                    for (int i = 0; i < Obj_Coolent.Length; i++)
                    {
                        UpdateMatInventory(Obj_Coolent[i], Inv_Coolent);
                    }
                }
                break;
        }
    }
   

    public void GiveItem(int OutputNum)
    {
        if (Inv_Output[OutputNum] != null && Inv_Output[OutputNum].Count > 0) 
        {
           
            GameObject nodeItem = Instantiate(item, OutputTransform.position, Quaternion.identity);
            NodeDestroy nodeDestroy = nodeItem.GetComponent<NodeDestroy>();
            nodeDestroy.Inv_Input = new Item { ItemType = Inv_Output[OutputNum].ItemType, Count = 1 };
            Item_Extract(OutputNum, 1, 1);
        }
    }


    public void Item_Extract(int Inv_Slot, int SlotType, int Count)
    {
        switch (SlotType) // 0 - Input, 1 - Output, 2 - Fuel, 3 - Coolent
        {
            case 0:
                Inv_Input[Inv_Slot] = SubtractItem(Inv_Input[Inv_Slot], Count);
                if (Inv_Slot < InputSlot.Length && InputSlot[Inv_Slot].Count != null)
                {
                    for (int j = 0; j < InputSlot[Inv_Slot].Count.Length; j++)
                    {
                        UpdateMatInventory(InputSlot[Inv_Slot].Count[j], Inv_Input[Inv_Slot]);
                    }
                }
                break;
            case 1:
                Inv_Output[Inv_Slot] = SubtractItem(Inv_Output[Inv_Slot], Count);
                if (Inv_Slot < OutputSlot.Length && OutputSlot[Inv_Slot].Count != null)
                {
                    for (int j = 0; j < OutputSlot[Inv_Slot].Count.Length; j++)
                    {
                        UpdateMatInventory(OutputSlot[Inv_Slot].Count[j], Inv_Output[Inv_Slot]);
                    }
                }
                break;
            case 2:
                Inv_Fuel = SubtractItem(Inv_Fuel, Count);
                if (FuelSlot != null)
                {
                    for (int i = 0; i < FuelSlot.Length; i++)
                    {
                        UpdateMatInventory(FuelSlot[i], Inv_Fuel);
                        if (FuelWick[i] != null) UpdateMatFill(FuelWick[i], (float)Inv_Fuel.Count / (float)Inv_Fuel.ItemType.MaxCount);
                    }
                }
                break;
            case 3:
                Inv_Coolent = SubtractItem(Inv_Coolent, Count);
                if (Inv_Coolent != null)
                {
                    for (int i = 0; i < Obj_Coolent.Length; i++)
                    {
                        UpdateMatInventory(Obj_Coolent[i], Inv_Coolent);
                    }
                }
                break;
        }
    }

    private IEnumerator ProcessItems(int i)
    {
        processing = true;
        //animator.SetBool("isActvie", true);
        while (itemData.nodeItemCount[i] > 0)
        {
            itemData.nodeItemCount[i]--;
            yield return new WaitForSeconds(5f);
            itemData.mixItemCount[i]++;
        }
        itemData.nodeItemCount[i] = 0;
        processing = false;
        //animator.SetBool("isActvie", false);
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
