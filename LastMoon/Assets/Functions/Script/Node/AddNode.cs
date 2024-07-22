using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AddNode : MonoBehaviour
{
    public GameObject uiPanel; // UI �г�

    // �浹 ó�� �� �̹��� ���� �� ��� ���� �ڵ�
    public Image node1;
    public Image node2;

    public Text[] nodeCounts_Text;
    public int[] nodeCounts;
    public int[] oldNodeCounts;

    public Sprite nodeAlpha;
    public Sprite[] nodeImage;

    private string oldNode;

    private int node1numeber = 0;
    private int node2numeber = 0;

    private bool nodeck1 = false;
    private bool nodeck2 = false;

    public Text node1_Text;
    public Text node2_Text;

    private string[] nodeName = { "node_Dirt", "node_Concrete", "node_Driftwood", "node_Sand", "node_Planks", "node_Scrap" };
    private int nodeAdd = 0;

    private PoiStart poiStartScript; // PoiStart ��ũ��Ʈ ����

    private void Start()
    {
        for (int i = 0; i < nodeCounts_Text.Length; i++)
        {
            nodeCounts[i] = int.Parse(nodeCounts_Text[i].text);
            oldNodeCounts[i] = int.Parse(nodeCounts_Text[i].text);
        }

        poiStartScript = FindObjectOfType<PoiStart>(); // PoiStart ��ũ��Ʈ ã��
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            for (int i = 0; i < nodeCounts_Text.Length; i++)
            {
                nodeCounts[i] = 10;
                oldNodeCounts[i] = 10;
                nodeCounts_Text[i].text = oldNodeCounts[i].ToString();
            }
        }
    }

    private void addnodeck()
    {
        if (nodeck1 && nodeck2)
        {
            for (int i = 0; i < nodeName.Length; i++)
            {
                if (i != node1numeber && i != node2numeber)
                {
                    nodeCounts[i] = oldNodeCounts[i];
                    nodeCounts_Text[i].text = oldNodeCounts[i].ToString();
                    nodeck1 = false;
                    nodeck2 = false;
                }
                else if (node1numeber == node2numeber)
                {
                    for (int j = 0; j < nodeName.Length; j++)
                    {
                        if (j != node1numeber)
                        {
                            nodeCounts[j] = oldNodeCounts[j];
                            nodeCounts_Text[j].text = oldNodeCounts[j].ToString();
                            nodeck1 = false;
                            nodeck2 = false;
                        }
                    }
                }
            }
        }
    }

    public void addNode1(Collider2D collision)
    {
        for (int i = 0; i < nodeName.Length; i++)
        {
            if (collision.name == nodeName[i])
            {
                node1.sprite = nodeImage[i];
                if (oldNode != nodeName[i])
                {
                    node1_Text.text = oldNodeCounts[i].ToString();
                }

                nodeCounts_Text[i].text = nodeCounts[i].ToString();
                node1numeber = i;
                nodeck1 = true;
                addnodeck();
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
                node2.sprite = nodeImage[i];
                if (oldNode != nodeName[i])
                {
                    node2_Text.text = oldNodeCounts[i].ToString();
                }
                nodeCounts_Text[i].text = nodeCounts[i].ToString();
                node2numeber = i;
                nodeck2 = true;
                addnodeck();
                break;
            }
        }
    }

    public void poi_Exit()
    {
        // PoiStart�� �ڷ�ƾ �簳
        if (poiStartScript != null)
        {
            poiStartScript.ResumeExecution();
        }
    }
}