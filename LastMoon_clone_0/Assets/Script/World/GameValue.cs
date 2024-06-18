using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class GameValue : MonoBehaviour
{
    static public int seed1;
    static public int seed2;
    static public int setMaxtime;
    static public string nickNumae;
    private NetworkManager networkManager;
    static public int Money_total;
    private static Text money;
    static public int Round = 0;
    static public bool RoundEnd;

    public static int Axe = 0;
    public static int Pickaxe = 0;
    public static int Shovel = 0;

    public static bool toolSwitching;


    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        Axe = MoneyController.Bt_Axe;
        Pickaxe = MoneyController.Bt_Pickaxe;
        Shovel = MoneyController.Bt_Shovel;
    }

    static public void setMoney()
    {
        money = GameObject.Find("Money_Text").GetComponent<Text>();
        SaveMoney(Money_total);
    }
    static public void SaveMoney(int saveMoney)
    {
        Money_total = saveMoney;
        money.text = Money_total.ToString();
    }
    static public void GetMomey(int getMomey)
    {
        Money_total = getMomey + Money_total;
        money.text = Money_total.ToString();
        
    }
    static public void UseMoney(int useMoney)
    {
        Money_total -= useMoney;
    }
    public void NickName(string name)
    {
        nickNumae = name;
    }
    public void seed(int a, int b)
    {
        seed1 = a;
        seed2 = 1;
        Debug.Log("seed: " + seed1);
        Debug.Log("seed: " + seed2);
    }
    public void setTimer (int maxtime)
    {
        setMaxtime = maxtime;
        Debug.Log("�ð�: " + setMaxtime);
    }
}
