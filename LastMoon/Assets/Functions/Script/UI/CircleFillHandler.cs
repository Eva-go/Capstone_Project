using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleFillHandler : MonoBehaviour
{
    private float fillValue = 1;
    public Image circleFillImage;
    public RectTransform handlerEdgeImage;
    public RectTransform fillHandler;
    public Text HP;
    public GameObject Hp;
    private float time;

    private bool isHpMax = false;
    // Start is called before the first frame update
    void Start()
    {
        Hp.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(!isHpMax)
        {
            if (fillValue < 100)
            {
                fillValue += time / 100;
            }
            PlayerController.Hp = ((int)fillValue);
            FillCircleValue(fillValue);
            if(PlayerController.Hp>=100)
            {
                PlayerController.Hp = 100;
                
                isHpMax = true;
            }
            
        }
        if (PlayerController.Hp <= 0)
        {
            FillCircleValue(0);
        }
        if(PlayerController.Hp>=100)
        {
            FillCircleValue(100);
        }

        HP.text = PlayerController.Hp.ToString();

        if(PlayerController.PreViewCam)
        {
            Hp.SetActive(false);
        }
    }

    void FillCircleValue(float value)
    {
        float fillAmount = (value / 100.0f);
        circleFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360;
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle);
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}
