using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    public Text Money;
    static public int total_Money;
    public int plus_Money;

    public static int Bt_Axe =0;
    public static int Bt_Pickaxe = 0;
    public static int Bt_Shovel = 0;

    public Text AxeMoney;
    public Text PickaxeMoney;
    public Text ShovelMoney;

    public Text Axetxt;
    public Text Pickaxetxt;
    public Text Shoveltext;
    private int int_AxeMoney;
    private int int_PickaxeMoney;
    private int int_ShovelMoney;

    public Button AxeBT;
    public Button PickaxeBT;
    public Button ShoveltBT;
        
    public Sprite[] ToolIcons;
    public Image AxeIcon, PickaxeIcon, ShovelIcon;

    private void Start()
    {
        Money.text = GameValue.MoneyUpdate().ToString();
        AxeMoney.text = "1000";
        PickaxeMoney.text = "1100";
        ShovelMoney.text = "1200";
        int_AxeMoney = int.Parse(AxeMoney.text);
        int_PickaxeMoney = int.Parse(PickaxeMoney.text);
        int_ShovelMoney = int.Parse(ShovelMoney.text);
        GameValue.toolSwitching = true;
        GameValue.insideUser = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameValue.GetMomey(1000);
        }
        Money.text = GameValue.MoneyUpdate().ToString();
        Debug.Log("MoneyUpdate" + GameValue.MoneyUpdate());

        if (int_AxeMoney > GameValue.MoneyUpdate())
        {
            AxeBT.interactable = false;
        }
        else
            AxeBT.interactable = true;



        if (int_PickaxeMoney > GameValue.MoneyUpdate())
        {
            PickaxeBT.interactable = false;
        }
        else
            PickaxeBT.interactable = true;


        if (int_ShovelMoney > GameValue.MoneyUpdate())
        {
            ShoveltBT.interactable = false;
        }
        else
            ShoveltBT.interactable = true;


        if (Bt_Axe==1)
        {
            AxeMoney.text = "2000";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Axetxt.text = "±Ý µµ³¢";
            AxeIcon.sprite = ToolIcons[0];
        }
        else if(Bt_Axe ==2)
        {
            AxeMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Axetxt.text = "SOLD OUT";
            AxeBT.interactable = false;
            //AxeIcon.sprite = ToolIcons[1];
        }
        if (Bt_Pickaxe == 1)
        {
            PickaxeMoney.text = "2100";
            int_PickaxeMoney = int.Parse(PickaxeMoney.text);
            Pickaxetxt.text = "±Ý °î±ªÀÌ";
            PickaxeIcon.sprite = ToolIcons[1];
        }
        else if (Bt_Pickaxe == 2)
        {
            PickaxeMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Pickaxetxt.text = "SOLD OUT";
            PickaxeBT.interactable = false;
            //PickaxeIcon.sprite = ToolIcons[10];
        }
        if (Bt_Shovel == 1)
        {
            ShovelMoney.text = "2200";
            int_ShovelMoney = int.Parse(ShovelMoney.text);
            Shoveltext.text = "±Ý »ð";
            ShovelIcon.sprite = ToolIcons[2];
        }
        else if (Bt_Shovel == 2)
        {
            ShovelMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Shoveltext.text = "SOLD OUT";
            ShoveltBT.interactable = false;
            //ShovelIcon.sprite = ToolIcons[11];
        }
    }


    public void AxeButton()
    {
        Bt_Axe += 1;
        total_Money -= int_AxeMoney;
        GameValue.UseMoney(int_AxeMoney);
    }

    public void PickaxeButton()
    {
        Bt_Pickaxe += 1;
        total_Money -= int_PickaxeMoney;
        GameValue.UseMoney(int_PickaxeMoney);
    }

    public void ShovelButton()
    {
        Bt_Shovel += 1;
        total_Money -= int_ShovelMoney;
        GameValue.UseMoney(int_ShovelMoney);
    }

}
