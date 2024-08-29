using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDestroy : MonoBehaviour
{
    public Item Inv_Input;
    private PoiController poiController;
    public GameObject ItemMaterial;

    public void Start()
    {
        ItemMaterial.GetComponent<MeshRenderer>().material = Inv_Input.ItemType.ItemLUM; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag=="Poi")
        {
            poiController =  collision.gameObject.GetComponent<PoiController>();
            poiController.TakeItem(Inv_Input);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Destroy(gameObject, 10f);
    }
}
