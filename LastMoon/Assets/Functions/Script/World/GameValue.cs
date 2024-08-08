using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameValue : MonoBehaviourPunCallbacks
{
    //싱글톤 디자인 패턴 사용인가?
    private NetworkManager networkManager;
    public static int MaxUser = 0;
    public static int insideUser = 0;
    public static bool inside = false;

    public static bool LocalPlayer=false;
    public static int seed1;
    public static int seed2;
    public static int setMaxtime;
    public static string nickName =null;
    public static int Money_total=0;
    public static Text money;

    public static int Round = 1;
    public static int MaxRound = 0;
    public static bool RoundEnd=false;

    public static int Axe = 0;
    public static int Pickaxe = 0;
    public static int Shovel = 0;

    public static bool toolSwitching =false;

    public static Camera mainCamera = null;

    public static Vector3 playerPos;

    public static bool lived = false;

    public static float WaveTimer;
    public static float WaveTimerMax;

    public static int MaxPlayer;
    public static int PlayerID;


    public static int[] usernodeItem = new int[6];
    public static int[] usermixItem = new int[6];

    public static int [] nodePrice = new int[6];
    public static int [] mixPrice = new int[6];

    public static int nodeMoney;
    public static int mixMoney;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
        MaxUser = 8;
        insideUser = 0;
        inside = false;
        nodeMoney = 0;
        mixMoney = 0;
        Round = 1;
    }


    public void Update()
    {
        Axe = MoneyController.Bt_Axe;
        Pickaxe = MoneyController.Bt_Pickaxe;
        Shovel = MoneyController.Bt_Shovel;
    }

    public static void getPrice(int i,int nodeCount, int mixCount)
    {
        nodePrice[i] = nodeCount;
        mixPrice[i] = mixCount;
    }

    public static void getItem()
    {
        for(int i=0;i<6;i++)
        {
            usernodeItem[i] = PlayerController.nodeSell[i];
            usermixItem[i] = PlayerController.mixSell[i];
        }
      
    }

    public static void Sell()
    {
        for(int i=0;i<nodePrice.Length;i++)
        {
            nodeMoney += usernodeItem[i] * nodePrice[i];
            mixMoney += usermixItem[i] * mixPrice[i];
        }
        Money_total += nodeMoney + mixMoney;
    }


    public static void setMoney()
    {
        money = GameObject.Find("Money_Text").GetComponent<Text>();
        SaveMoney(Money_total);
    }
    public static void SaveMoney(int saveMoney)
    {
        Money_total = saveMoney;
        money.text = Money_total.ToString();
    }
    public static void GetMomey(int getMomey)
    {
        Money_total = getMomey + Money_total;
        money.text = Money_total.ToString();
    }

    public static void UseMoney(int useMoney)
    {
        Money_total -= useMoney;
    }
    public void NickName(string name)
    {
        nickName = name;
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
    }


}
