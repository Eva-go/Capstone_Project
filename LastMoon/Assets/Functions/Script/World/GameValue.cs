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
    //�̱��� ������ ���� ����ΰ�?
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
    public static string NodeName =null;
    public static int[] NodeCount =null;

    public static Camera mainCamera = null;

    public static Vector3 playerPos;

    public static bool lived = false;

    public static float WaveTimer;

    public int userNumber;




    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
        MaxUser = 8;
        insideUser = 0;
        inside = false;
    }


    public void Update()
    {
        Axe = MoneyController.Bt_Axe;
        Pickaxe = MoneyController.Bt_Pickaxe;
        Shovel = MoneyController.Bt_Shovel;
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
