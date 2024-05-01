using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UI_manager : MonoBehaviour
{
    public Sprite[] buttonImage;
    public Button[] buttons;
    private void Start()
    {
        for(int i=0;i<buttons.Length;i++)
        {
            if (i < buttonImage.Length && buttonImage[i] != null)
            {
                buttons[i].image.sprite = buttonImage[i];
            }
            else
                Debug.Log("¿¡·¯");
        }
    }

    public void OnButtonCilck()
    {

    }

  

}
