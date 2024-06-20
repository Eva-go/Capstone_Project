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
        // ���콺 ��ġ���� Raycast�� ����Ͽ� UI ��� �浹 �˻�
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("�浹");
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