using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public string Name;
    public GameObject Object_prefab; //��ġ�� ������ go_Prefab
    public GameObject Object_prview_Prefab; // �̸����� ������ go_PreviewPrefab
}


public class CraftMaunal : MonoBehaviour
{
    //���º���
    private bool isActivated = false;
    private bool isPreViewActivated = false;

    [SerializeField]
    private GameObject Inventory_UI; //go_BaseUI; �⺻ ���̽� UI

    [SerializeField]
    private Craft[] craft_Tab; //���๰ ��

    private GameObject Object_Preview; //�̸����� �������� ���� ����

    private Transform tf_player;
    private PlayerController playerController;
    //Raycast ����
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;
   
    private void Start()
    {
       
    }

    public void SlotClick(int _slotNumber)
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        tf_player = playerController.transform;
        Object_Preview = Instantiate(craft_Tab[_slotNumber].Object_prview_Prefab, tf_player.transform.position + tf_player.transform.forward, Quaternion.identity);
        isPreViewActivated = true;
        Inventory_UI.SetActive(false);
    }


   

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.I))
            Inventory();//Winodws

        if (isPreViewActivated)
            previewPositionUpdate();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }


    private void previewPositionUpdate()
    {
        if (Physics.Raycast(tf_player.transform.position, tf_player.transform.forward,out hitInfo,range,layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                Object_Preview.transform.position = _location;
            }
        }
    }
    public void Cancel()
    {
        if(isPreViewActivated)
            Destroy(Object_Preview);
        isActivated = false;
        isPreViewActivated = false;
        Object_Preview = null;
        Inventory_UI.SetActive(false);
    }

    private void Inventory()
    {
        if (!isActivated)
            OpenInventory();
        else
            CloseInventory();
    }

    private void OpenInventory()
    {
        isActivated = true;
        Inventory_UI.SetActive(true);
    }

    private void CloseInventory()
    {
        isActivated = false;
        Inventory_UI.SetActive(false);
    }
}
