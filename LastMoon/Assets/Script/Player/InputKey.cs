using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKey : MonoBehaviour
{
    // Start is called before the first frame update
    private int KeyTabCode = 0;
    private bool Inventory = false;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Player_UI();
    }


    public void Player_UI()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) 
        {
            KeyTabCode++;
            switch (KeyTabCode%3)
            {
                case 0:
                    GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(true);
                    GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(false);
                    GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(false);
                    if(Inventory)
                    {
                        GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(true);
                        GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(false);
                        GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(false);
                    }
                    break;
                case 1:
                    GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(true);
                    GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(false);
                    if (Inventory)
                    {
                        GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(false);
                        GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(true);
                        GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(false);
                    }
                    break;
                case 2:
                    GameObject.Find("Resources").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Resources").transform.GetChild(2).gameObject.SetActive(false);
                    GameObject.Find("Resources").transform.GetChild(3).gameObject.SetActive(true);
                    if (Inventory)
                    {
                        GameObject.Find("Tab").transform.GetChild(0).gameObject.SetActive(false);
                        GameObject.Find("Tab").transform.GetChild(1).gameObject.SetActive(false);
                        GameObject.Find("Tab").transform.GetChild(2).gameObject.SetActive(true);
                    }
                    break;  
            }
        }
    }
}
