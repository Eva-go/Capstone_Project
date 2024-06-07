using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideBar : MonoBehaviour
{
    public GameObject insideObject;
    private void Start()
    {
        insideObject.SetActive(false);
    }

    private void Update()
    {
        InsideActive();

    }

    public void InsideActive()
    {
        insideObject.SetActive(PlayerController.insideActive);
        if(PlayerController.PreViewCam)
        {
            insideObject.SetActive(false);
        }
    }

}
