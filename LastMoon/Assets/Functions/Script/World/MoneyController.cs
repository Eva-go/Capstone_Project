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

    private void Start()
    {
        GameValue.Sell();
        Debug.Log("µ·" + GameValue.Money_total);
        GameValue.setMoney();
       
        total_Money = GameValue.Money_total;
        Money.text = total_Money.ToString();
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
        if (Input.GetKey("escape"))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameValue.GetMomey(1000);
        }

        if (int_AxeMoney > total_Money)
        {
            AxeBT.interactable = false;
        }
        else
            AxeBT.interactable = true;



        if (int_PickaxeMoney > total_Money)
        {
            PickaxeBT.interactable = false;
        }
        else
            PickaxeBT.interactable = true;


        if (int_ShovelMoney > total_Money)
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
        }
        else if(Bt_Axe ==2)
        {
            AxeMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Axetxt.text = "SOLD OUT";
            AxeBT.interactable = false;
         }
        if (Bt_Pickaxe == 1)
        {
            PickaxeMoney.text = "2100";
            int_PickaxeMoney = int.Parse(PickaxeMoney.text);
            Pickaxetxt.text = "±Ý °î±ªÀÌ";
        }
        else if (Bt_Pickaxe == 2)
        {
            PickaxeMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Pickaxetxt.text = "SOLD OUT";
            PickaxeBT.interactable = false;
        }
        if (Bt_Shovel == 1)
        {
            ShovelMoney.text = "2200";
            int_ShovelMoney = int.Parse(ShovelMoney.text);
            Shoveltext.text = "±Ý »ð";
        }
        else if (Bt_Shovel == 2)
        {
            ShovelMoney.text = "0";
            int_AxeMoney = int.Parse(AxeMoney.text);
            Shoveltext.text = "SOLD OUT";
            ShoveltBT.interactable = false;
        }
    }


    public void AxeButton()
    {
        Bt_Axe += 1;
        total_Money -= int_AxeMoney;
        Money.text = total_Money.ToString();
        GameValue.UseMoney(int_AxeMoney);
    }

    public void PickaxeButton()
    {
        Bt_Pickaxe += 1;
        total_Money -= int_PickaxeMoney;
        Money.text = total_Money.ToString();
        GameValue.UseMoney(int_PickaxeMoney);
    }

    public void ShovelButton()
    {
        Bt_Shovel += 1;
        total_Money -= int_ShovelMoney;
        Money.text = total_Money.ToString();
        GameValue.UseMoney(int_ShovelMoney);
    }

}
