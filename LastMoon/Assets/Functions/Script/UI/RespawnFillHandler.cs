using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RespawnFillHandler : MonoBehaviour
{
    private float fillTime = 0.0f;
    private float fillDuration = 5.0f;
    public static float fillValue = 0;
    private bool hasFilled = false; // Added to track if fillValue reached 100

    public Image InsideFillImage;
    public RectTransform handlerEdgeImage;
    public RectTransform fillHandler;
    public GameObject insidegameObject;

    public PlayerController player;
    public PhotonView pv;

    //tick  관련 변수
    public int tick;
    public int tickMax;
    public bool isConstructing;




    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            player = GameObject.Find(GameValue.nickName).GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (gameObject.activeSelf == true)//&&player.respawnTick)
            {
             
            }

        }
        
    }
        

    public void FillCircleValue(float value)
    {
        float fillAmount = (value / 100.0f);
        InsideFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360;
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle);
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}