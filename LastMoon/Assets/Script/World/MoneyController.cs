using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{

    public Text Money;
    static public int total_Money;
    private void Start()
    {
        total_Money = GameValue.Money_total;
        Money.text = total_Money.ToString();
    }
    private void Update()
    {
        GameValue.SaveMoney(total_Money);
    }

}
