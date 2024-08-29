using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_OpenTab : MonoBehaviour
{
    public void Open_CloseTab(GameObject Tab)
    {
        Tab.SetActive(!Tab.activeSelf);
    }
    public void OpenTab(GameObject Tab)
    {
        Tab.SetActive(true);
    }
}
