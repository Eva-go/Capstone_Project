using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    private bool inEscapeMenu;
    public GameObject Menu;

    void Start()
    {
        inEscapeMenu = false;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inEscapeMenu)
            {
                EscapeMenuClose();
            }
            else
            {
                EscapeMenuOpen();
            }
        }
    }

    public void EscapeMenuOpen()
    {
        inEscapeMenu = true;
        Menu.SetActive(true);
    }

    public void EscapeMenuClose()
    {
        inEscapeMenu = false;
        Menu.SetActive(false);
    }


    public void EscapeMenuQuit()
    {
        Application.Quit();
    }
}
