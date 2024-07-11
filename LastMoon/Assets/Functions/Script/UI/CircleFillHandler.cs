using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleFillHandler : MonoBehaviour
{
    private float playerHP_UI = 1;
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
        playerHP_UI = PlayerController.Hp;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(!isHpMax)
        {
            FillCircleValue(PlayerController.Hp); 
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
