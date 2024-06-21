using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PoiController : MonoBehaviour
{
    public Sprite[] nodeImage;
    private string[] nodeName = { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap" };

    void Update()
    {
        // 마우스 위치에서 Raycast를 사용하여 UI 요소 충돌 검사
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("충돌");
        if (other.gameObject.tag == "Misc")
        {
            for (int i = 0; i < nodeName.Length; i++)
            {
                if (other.gameObject.name == nodeName[i])
                {
                    gameObject.GetComponent<Image>().sprite = nodeImage[i];
                    break;
                }
            }
        }

        
       
    }
}