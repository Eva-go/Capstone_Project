using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poi : MonoBehaviour
{
    public GameObject PoiUI;
    private void Start()
    {
        PoiUI.SetActive(PlayerController.Poi);
    }

    private void Update()
    {
        PoiActive();
    }

    public void PoiActive()
    {
        PoiUI.SetActive(PlayerController.Poi);
        if(PlayerController.Poi)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    public void Bt_PoiExt()
    {
        PoiUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerController.Poi = false;
    }

}
