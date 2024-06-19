using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MiscTab : MonoBehaviour
{
    public GameObject[] nodes;
    public Text[] nodesCount;
    public int[] count;
    private void Start()
    {
        for(int i=0; i<nodesCount.Length; i++)
        {
            nodesCount[i].text = "0";
            count[i] = 0;
        }
    }

    private void Update()
    {
        nodeCountUpdate();
        Die();
    }


    public void nodeCountUpdate()
    {
        for (int i = 0; i < nodesCount.Length; i++)
        {
            if(nodes[i].name+"(Clone)" == GameValue.NodeName)
            {
                count[i]++;
                nodesCount[i].text = count[i].ToString();
                GameValue.GetNode("");
            }
        }
    }

    public void Die()
    {
        if(PlayerController.Hp==0)
        {
            for (int i = 0; i < nodesCount.Length; i++)
            {
                nodesCount[i].text = "0";
            }
        }
    }
}