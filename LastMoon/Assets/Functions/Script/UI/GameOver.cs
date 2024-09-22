using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

using UnityEngine.Playables;

public class GameOver : MonoBehaviour
{
    public GameObject Alive;
    public GameObject Die;
    public GameObject Button;
    public bool Winner;

    public PlayableDirector EndingTimeline;

    // Start is called before the first frame update
    void Start()
    {
        Alive.SetActive(false);
        //Winner = GameValue.is_Winner;
        if(Winner)
        {
            EndingTimeline.Play();
            Die.SetActive(false);
            Alive.SetActive(false);
            //Button.SetActive(false);
        }
        else
        {
            Die.SetActive(true);
            Button.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.Confined;

        GameValue.exit = false;

        GameValue.MaxUser = 8;
        GameValue.insideUser = 0;
        GameValue.inside = false;
        GameValue.nodeMoney = 0;
        GameValue.mixMoney = 0;
        GameValue.is_Winner = false;


        GameValue.LocalPlayer = false;
        GameValue.seed1 = 0;
        GameValue.seed2 = 0;
        GameValue.setMaxtime=0;
        GameValue.nickName = null;
        GameValue.Money_total = 0;

        GameValue.Round = 1;
        GameValue.MaxRound = 0;
        GameValue.RoundEnd = false;

        GameValue.Axe = 0;
        GameValue.Pickaxe = 0;
        GameValue.Shovel = 0;

        GameValue.toolSwitching = false;

        GameValue.mainCamera = null;


        GameValue.lived = false;


        //exit
        GameValue.exit=false;
        GameValue.TideCycle = 1;

        GameValue.TideChange = false;
        GameValue.TideChangeProgress = 0;
}


    public void EndingMenu()
    {
        Alive.SetActive(true);
        Button.SetActive(true);
        EndingTimeline.Stop();
    }


    public void Restart_BT()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 서버와의 연결을 끊고 씬 전환
        }
        GameValue.exit = true;
        PhotonNetwork.LoadLevel("RoomMeun");
    }

}
