using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InsideFillHandler : MonoBehaviour
{
    private float fillTime = 0.0f;
    private float fillDuration = 5.0f;
    private float fillValue = 0;
    public Image InsideFillImage;
    public  RectTransform handlerEdgeImage;
    public  RectTransform fillHandler;
    public GameObject insidegameObject;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.insideActive)
        {
            fillTime += Time.deltaTime;
            if (fillValue < 100)
            {
                fillValue += fillTime / 100*25;
            }
            else if(fillValue >=100)
            {
                fillValue = 100;
                PlayerController.PreViewCam = true;
                
            }
            FillCircleValue(fillValue);
        }
        else if(!PlayerController.insideActive)
        {
            fillTime = 0;
            fillValue = 0;
            FillCircleValue(fillValue);
        }
    }
    public  void FillCircleValue(float value)
    {
        float fillAmount = (value / 100.0f);
        InsideFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360;
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle);
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}
