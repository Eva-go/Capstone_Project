using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InsideFillHandler : MonoBehaviour
{
    private float fillTime = 0.0f;
    private float fillDuration = 5.0f;
    public  static float fillValue = 0;
    private bool hasFilled = false; // Added to track if fillValue reached 100

    public Image InsideFillImage;
    public RectTransform handlerEdgeImage;
    public RectTransform fillHandler;
    public GameObject insidegameObject;

    public PlayerController player;
    public PhotonView pv;

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
        if (PlayerController.insideActive)
        {
            fillTime += Time.deltaTime;
            if (fillValue < 100)
            {
                fillValue += fillTime / 100 * 25;
            }
            else if (fillValue >= 100)
            {
                if (!hasFilled && pv.IsMine)
                {
                    fillValue = 100;
                    PlayerController.Hp = 100;
                    //player.inside++;
                    //player.inside = player.inside % 4;
                    //player.oldpos = true;
                    hasFilled = true; // Set flag to true to prevent multiple updates
                }
            }
            FillCircleValue(fillValue);
        }
        else
        {
            fillTime = 0;
            fillValue = 0;
            hasFilled = false; // Reset flag when not inside
            FillCircleValue(fillValue);
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