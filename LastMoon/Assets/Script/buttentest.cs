using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttentest : MonoBehaviour
{
    Button bt;
    void Start()
    {
        bt = GetComponent<Button>();
        bt.onClick.AddListener(ButtonClick);
    }
    public void ButtonClick()
    {
        Debug.Log("Å¬¸¯");
    }
   
}
