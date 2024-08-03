using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoiSpawn : MonoBehaviour
{
    public GameObject[] PoiTab;
    private PlayerController playerController;
    private Transform tf_player;

    void Start()
    { 
        playerController = gameObject.GetComponent<PlayerController>();

        GameObject poiLists = GameObject.FindWithTag("PoiList").transform.Find("PoiListBG").gameObject;
        
        for (int i = 0; i < PoiTab.Length; i++)
        {
            PoiTab[i] = poiLists.transform.GetChild(i).gameObject;
            Debug.Log("PoiTab[" + i + "] set to: " + PoiTab[i].name);
        }

    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void SlotClick(int _slotNumber)
    {
        tf_player = playerController.theCamera.transform;
    }
}