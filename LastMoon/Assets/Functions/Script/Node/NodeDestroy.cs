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
            if (Inv_Input.ItemType.ItemLUM !=null)
            {
                ItemMaterial.GetComponent<MeshRenderer>().material = Inv_Input.ItemType.ItemLUM;
                if (Inv_Input.ItemType.ItemObject != null)
                {
                    ItemMaterial.GetComponent<MeshRenderer>().enabled = false;
                    nodeItem = Instantiate(Inv_Input.ItemType.ItemObject, gameObject.transform);
                }
            }
        }
        if (Inv_Input.ItemType.Liquid)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
           
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.PlayerInventory.AddItem(new Item { ItemType = Inv_Input.ItemType, Count = Inv_Input.Count });
            playerController.InvokeInventoryChanged();
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Poi")
        {
            poiController = collision.gameObject.GetComponent<PoiController>();
            poiController.TakeItem(Inv_Input);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag != "Item")
        {
            if (Inv_Input.ItemType.Liquid)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, 10f);
            }

        }
    }

    private void Update()
    {
        //Destroy(gameObject, 10f);
    }
}
