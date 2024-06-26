using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameValue : MonoBehaviour
{
    private NetworkManager networkManager;
    public static int MaxUser = 0;
    public static int insideUser = 0;
    public static bool inside = false;

    public static bool LocalPlayer=false;
    public static int seed1;
    public static int seed2;
    public static int setMaxtime;
    public static string nickNumae =null;
    public static int Money_total=0;
    public static Text money;
    public static int[] PlayerID_money;

    public static int Round = 0;
    public static bool RoundEnd=false;

    public static int Axe = 0;
    public static int Pickaxe = 0;
    public static int Shovel = 0;

    public static bool toolSwitching =false;
    public static string NodeName =null;
    public static int[] NodeCount =null;

    public static Camera mainCamera = null;

    public static Vector3 playerPos;

    public static bool lived = false;

    public static float WaveTimer;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
        MaxUser = 0;
        insideUser = 0;
        inside = false;
    }


    public void Update()
    {
        Axe = MoneyController.Bt_Axe;
        Pickaxe = MoneyController.Bt_Pickaxe;
        Shovel = MoneyController.Bt_Shovel;
        Debug.Log("¾ÈÂÊ :" + insideUser + " MAX : " + MaxUser);
    }

    public static void setPlayerIDMoney()
    {
        for(int i=0; i< MaxUser;i++)
        {
            PlayerID_money[i] = 0;
        }
    }

    public static void getPlayerIDMoney()
    {
        int i = NetworkManager.PlayerID;
        PlayerID_money[i] = Money_total;
        for(int n=0; i<PhotonNetwork.PlayerList.Length; n++)
        {
            
        }
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

    public static void GetNode(string nodeName)
    {
        NodeName = nodeName;
    }

    public static void UseMoney(int useMoney)
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
    }


}
