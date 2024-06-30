using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class RaningUpdate : MonoBehaviour
{
    public GameObject[] Raning;
    public Text[] NickName;
    public Text[] TotalMoney;
    private GameObject value;

    // Start is called before the first frame update
    void Start()
    {
        
        value = GameObject.Find("GameValue");
        for(int i=0; i<8;i++)
        {
            Raning[i].gameObject.SetActive(false);
        }
        for(int i=0; i<GameValue.MaxUser;i++)
        {
            Raning[i].SetActive(true);
            
        }
        value.GetComponent<GameValue>().RankingUpdate();
       
        for (int i=0; i<GameValue.MaxUser;i++)
        {
            NickName[i].text = GameValue.PlayerName[i].ToString();
            TotalMoney[i].text = GameValue.PlayerID_money[i].ToString();
        }

       

    }

    // Update is called once per frame
    void Update()
    {

    }
}
