using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDestroy : MonoBehaviour
{
    public Item Inv_Input;
    private PoiController poiController;
    public GameObject ItemMaterial;
    private PlayerController playerController;

    public void Start()
    {
        GameObject nodeItem;
        if (Inv_Input != null)
        {
            if(Inv_Input.ItemType.ItemLUM !=null)
            {
                ItemMaterial.GetComponent<MeshRenderer>().material = Inv_Input.ItemType.ItemLUM;
                if (Inv_Input.ItemType.ItemObject != null)
                {
                    ItemMaterial.GetComponent<MeshRenderer>().enabled = false;
                    nodeItem = Instantiate(Inv_Input.ItemType.ItemObject, gameObject.transform);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�浹" + collision.gameObject.name);
        Debug.Log("�浹" + collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("�÷��̾� �浹");
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.PlayerInventory.AddItem(Inv_Input);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag=="Poi")
        {
            Debug.Log("�浹1");
            poiController =  collision.gameObject.GetComponent<PoiController>();
            poiController.TakeItem(Inv_Input);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag!="Poi" && Inv_Input.ItemType.Liquid)
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag!="Poi"&&!Inv_Input.ItemType.Liquid)
        {
            Destroy(gameObject, 10f);
        }
      
    }

    private void Update()
    {
        //Destroy(gameObject, 10f);
    }
}
