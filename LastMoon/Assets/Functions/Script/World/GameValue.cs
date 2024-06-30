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
    public static int[] PlayerID_money =null;
    public static string[] PlayerName =null;

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
        MaxUser = 8;
        insideUser = 0;
        inside = false;
        PlayerID_money = new int[MaxUser];
        PlayerName = new string[MaxUser];
    }


    public void Update()
    {
        Axe = MoneyController.Bt_Axe;
        Pickaxe = MoneyController.Bt_Pickaxe;
        Shovel = MoneyController.Bt_Shovel;
    }
    public void RankingUpdate()
    {
        if (photonView != null)
        {
            photonView.RPC("RPC_RankingMoney", RpcTarget.AllBuffered, Money_total, nickName);
        }
        else
        {
            Debug.LogError("PhotonView가 null입니다.");
        }
    }

    [PunRPC]
    void RPC_RankingMoney(int Money,string name)
    {
        int i = NetworkManager.PlayerID;
        Debug.Log("랭킹" + i+"돈"+ Money+"이름"+name);
        PlayerID_money[i] = Money;
        PlayerName[i] = name;
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
