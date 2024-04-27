using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Selected : MonoBehaviour
{
    private Button Consumable_Selected_Btn, Construction_Selected_Btn, Misc_Selected_Btn;
    private void Awake()
    {
        //GameObject.Find("CubeParent").transform.GetChild(0).gameObject.SetActive(true);
        //Consumable_Selected = GameObject.Find("Consumable_Selected");
        //Construction_Selected = GameObject.Find("Construction_Selected");
        //Misc_Selected = GameObject.Find("Misc_Selected");

        //��ư ��Ȱ��ȭ
        //btn = item.transform.Find("Button").GetComponent<Button>();
        Consumable_Selected_Btn = GameObject.Find("Selected").transform.GetChild(0).GetComponent<Button>();
        Construction_Selected_Btn = GameObject.Find("Selected").transform.GetChild(1).GetComponent<Button>();
        Misc_Selected_Btn = GameObject.Find("Selected").transform.GetChild(2).GetComponent<Button>();
        //Consumable_Selected_Btn = gameObject.transform.Find("Button1").GetComponent<Button>();
        //Construction_Selected_Btn = gameObject.transform.Find("Button2").GetComponent<Button>();
        //Misc_Selected_Btn = gameObject.transform.Find("Button3").GetComponent<Button>();
        //btn.interactable = false; // ��ư Ŭ���� ��Ȱ��
        //btn.interactable = true; // ��ư Ŭ���� 
    }
    public void Consumable_Selected_OnClick()
    {
        GameObject.Find("Button1").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Button2").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Button3").transform.GetChild(0).gameObject.SetActive(false);
        //��ư Ȱ��ȭ ��Ȱ��ȭ
        Consumable_Selected_Btn.interactable = false;
        Construction_Selected_Btn.interactable = true;
        Misc_Selected_Btn.interactable = true;
    }
    public void Construction_Selected_OnClick()
    {
        GameObject.Find("Button1").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Button2").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Button3").transform.GetChild(0).gameObject.SetActive(false);
        //��ư Ȱ��ȭ ��Ȱ��ȭ
        Consumable_Selected_Btn.interactable = true;
        Construction_Selected_Btn.interactable = false;
        Misc_Selected_Btn.interactable = true;

    }
    public void Misc_Selected_OnClick()
    {
        GameObject.Find("Button1").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Button2").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Button3").transform.GetChild(0).gameObject.SetActive(true);
        //��ư Ȱ��ȭ ��Ȱ��ȭ
        Consumable_Selected_Btn.interactable = true;
        Construction_Selected_Btn.interactable = true;
        Misc_Selected_Btn.interactable = false;
    }
}
