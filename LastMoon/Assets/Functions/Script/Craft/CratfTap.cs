using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CratfTap : MonoBehaviour
{
    public GameObject CratfTapPrefab;
    private GameObject CratfTapInstance;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            ToggleCartfTap();
        }
    }

    private void ToggleCartfTap()
    {
        if(CratfTapInstance == null)
        {
            OpenCartfTap();
        }
        else
        {
            CloseCartfTap();
        }
    }

    private void OpenCartfTap()
    {
        if(CratfTapPrefab != null)
        {
            CratfTapInstance = Instantiate(CratfTapPrefab);

            CratfTapInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        else
        {
            Debug.LogError("CratfTapInstance Null");
        }
    }
    private void CloseCartfTap()
    {
        if(CratfTapInstance != null )
        {
            Destroy(CratfTapInstance);
            CratfTapInstance = null;
        }
    }
}
