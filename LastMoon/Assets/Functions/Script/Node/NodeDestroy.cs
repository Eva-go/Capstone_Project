using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Poi")
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Destroy(gameObject, 10f);
    }
}
