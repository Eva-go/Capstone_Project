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


    private void TimeTickSystem_OnTick(object sender, TickTimer.OnTickEventArgs e)
    {
        if (isConstructing)
        {
            tick = e.tick % tickMax;
            if (tick >= tickMax - 1)
            {
                if (PlayerController.RespawnAcive)
                {
                    if (fillValue < 100)
                    {
                        fillValue += tick;
                    }
                    else if (fillValue >= 100)
                    {
                        if (!hasFilled && pv.IsMine)
                        {
                            fillValue = 100;
                            hasFilled = true; // Set flag to true to prevent multiple updates
                        }
                    }
                    FillCircleValue(fillValue);
                }
                else
                {
                    tick = 0;
                    fillValue = 0;
                    hasFilled = false; // Reset flag when not inside
                    FillCircleValue(fillValue);
                }
            }
            else
            {

                //Debug.Log("Tick tick true" + tick + ":"+tickMax+" "+ PhotonNetwork.Time);
            }

        }
    }

    public void tick_ck(int ticksToConstruct)
    {

        if (!isConstructing)
        {
            tickMax = ticksToConstruct;
            isConstructing = true;
            TickTimer.OnTick += TimeTickSystem_OnTick;
        }

    }


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
        tick_ck(10);
       
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