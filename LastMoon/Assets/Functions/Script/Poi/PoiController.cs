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

    public GameObject[] StationBases;
    public GameObject[] StationFixes;
    public GameObject[] StationAuxes;
    public ObjArray[] StationCons;

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

    public ScriptableObject_Station[] SelectableRecipes;
    public ScriptableObject_Station SelectedRecipe;

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

    //tick  관련 변수
    public int tick;
    public int tickMax;
    public bool isConstructing;
    public bool stop;



    //레시피 관련 변수
    private int slotNumber; // 현재 슬롯 번호

    //파괴 변수
    public int hp = 3;


    //출력 변수
    /*
    public GameObject itme;
    public Transform OutputTransform;

    public bool isOutput;
    public bool Ountput_stop;
    public int tickMaxOUtput;
    public int OutputTick;
    public bool test_ck;
     */
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
        hp = 3;
        StationProgress = 0;
        Inv_Fuel = new Item { ItemType = new ScriptableObject_Item {}, Count = 0 };
        Inv_Coolent = new Item { ItemType = new ScriptableObject_Item { }, Count = 0 };
        //test_ck = false;

        isConstructing = false;

        Activation = false;
        Heating = false;
        Refilling = true;
        CoolentDrain = 0;
    }

    private void Update()
    {
        ActivationEffect();
        tick_ck(1);

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
        for(int i=0; i< SelectableRecipes.Length; i++)
        {
            if(i == _slotNumber)
            {
                SelectedRecipe = SelectableRecipes[i];
            }
        }
    }
    private void TimeTickSystem_OnTick(object sender, TickTimer.OnTickEventArgs e)
    {
        if (isConstructing&&!stop)
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
                //Ountput(0);
                isConstructing = false;
            }
            else
            {

                //Debug.Log("Tick tick true" + tick + ":"+tickMax+" "+ PhotonNetwork.Time);
            }

        }
    }
    /*
    private void TimeTickSystem_OnTick_OutPut(object sender, TickTimer.OnTickEventArgs e)
    {
        if (isOutput && Ountput_stop)
        {
            OutputTick = e.tick % tickMaxOUtput;
            if (OutputTick >= tickMaxOUtput - 1)
            {
                Ountput(0);
                isOutput = false;
            }
            else
            {

                //Debug.Log("Tick tick true" + tick + ":"+tickMax+" "+ PhotonNetwork.Time);
            }

        }
    }
     */

    /*
    public void Ountput(int index)
    {
        Instantiate(itme, OutputTransform.position, Quaternion.identity);
    }
     */

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

    public void tick_ck(int ticksToConstruct)
    {
        if (!isConstructing)
        {
            tickMax = ticksToConstruct;
            isConstructing = true;
            TickTimer.OnTick += TimeTickSystem_OnTick;
        }
    }
    /*
    public void tick_OutPut(int ticksToConstruct)
    {
        if (!isOutput)
        {
            tickMaxOUtput = ticksToConstruct;
            isOutput = true;
            TickTimer.OnTick += TimeTickSystem_OnTick_OutPut;
        }
    }
     */



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
            StationProgress++;
            if (StationProgress >= SelectedRecipe.ProgressTime)
            {
                StationProgress = 0;
                if (hp < 3) hp++;
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


    public void InputItem(int ItemRequireCount)
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

    public void ExtractItem(int Inv_Slot)
    {
        Inv_Output[Inv_Slot].ClearItem();
        if (Inv_Slot < OutputSlot.Length && OutputSlot[Inv_Slot].Count != null)
        {
            for (int i = 0; i < OutputSlot[Inv_Slot].Count.Length; i++)
            {
                UpdateMatInventory(OutputSlot[Inv_Slot].Count[i], Inv_Output[Inv_Slot]);
            }
        }
    }

    [PunRPC]
    public void ReceiveData()
    {
        //InputItem();

        //=== Old Recipe -V
        /*
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
         */
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
