using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class ToolController : MonoBehaviour
{

    public GameObject[] Tools;
    public GameObject[] ToolSwitching;
    private int selectedToolIndex = 0;


    private void Start()
    {
        GameObject ToolInstance = Instantiate(Tools[0], gameObject.transform.position, gameObject.transform.rotation);
        ToolInstance.transform.SetParent(gameObject.transform);
    }
    private void Update()
    {
        Switching();
    }
    private void Switching()
    {
        int previousSelectedWeaponIndex = selectedToolIndex;
        if (GameValue.Axe == 1)
        {
            Tools[0] = ToolSwitching[0];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 1;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Axe == 2)
        {
            Tools[0] = ToolSwitching[3];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 1;
                GameValue.toolSwitching = false;
            }

        }
        if (GameValue.Pickaxe == 1)
        {
            Tools[1] = ToolSwitching[1];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 2;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Pickaxe == 2)
        {
            Tools[1] = ToolSwitching[4];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 2;
                GameValue.toolSwitching = false;
            }

        }
        if (GameValue.Shovel == 1)
        {
            Tools[2] = ToolSwitching[2];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 0;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Shovel == 2)
        {
            Tools[2] = ToolSwitching[5];
            if (GameValue.toolSwitching)
            {
                selectedToolIndex = 0;
                GameValue.toolSwitching = false;
            }
        }

        // 무기 번호 키로 무기 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedToolIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && Tools.Length >= 2)
        {
            selectedToolIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && Tools.Length >= 3)
        {
            selectedToolIndex = 2;
        }

        // 무기 교체가 필요하면 새로운 무기를 장착
        if (previousSelectedWeaponIndex != selectedToolIndex)
        {
            EquipWeapon(selectedToolIndex);
        }
    }

    private void EquipWeapon(int index)
    {
        // 기존에 장착된 무기 비활성화
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        // 새로운 무기 인스턴스 생성 및 장착
        GameObject weaponInstance = Instantiate(Tools[index], gameObject.transform.position, gameObject.transform.rotation);
        weaponInstance.transform.SetParent(gameObject.transform);
    }
}