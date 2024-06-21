using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AddNode : MonoBehaviour
{
    //충돌처리후 이미지 변경 및 노드 갯수 코드
    public Image node1;
    public Image node2;

    public Sprite nodeAlpha;
    public Text[] nodeCounts;
    public Sprite[] nodeImage;
    private string[] nodeName = { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap", "Alpha" };
    private int nodeAdd = 0;
    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public void addNode1(Collider2D collision)
    {
        for (int i = 0; i < nodeName.Length; i++)
        {
            if (collision.name == nodeName[i])
            {
                nodeAdd = i;
                node1.sprite = nodeImage[i];
                break;
            }
        
        }
    }

    public void addNode2(Collider2D collision)
    {
        for (int i = 0; i < nodeName.Length; i++)
        {
            if (collision.name == nodeName[i])
            {
                nodeAdd = i;
                node2.sprite = nodeImage[i];
                break;
            }

        }
    }

    public void poi_Exit()
    {
        nodeAdd = 7;
        gameObject.GetComponent<Image>().sprite = nodeAlpha;
    }
}