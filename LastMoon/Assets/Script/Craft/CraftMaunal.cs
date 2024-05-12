using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[System.Serializable]
public class Craft
{
    public string Name;
    public GameObject Object_prefab; //설치될 프리팹 go_Prefab
    public GameObject Object_prview_Prefab; // 미리보기 프리뷰 go_PreviewPrefab
}


public class CraftMaunal : MonoBehaviour
{
    //상태변수
    public static bool isActivated = false;
    public static bool isPreViewActivated = false;

    private int slotNumber;

    [SerializeField]
    private GameObject Inventory_UI; //go_BaseUI; 기본 베이스 UI

    [SerializeField]
    private Craft[] craft_Tab; //건축물 탭

    private GameObject Object_Preview; //미리보기 프리팹을 담을 변수
    private GameObject Object_Prefab;

    private Transform tf_player;
    private PlayerController playerController;
    //Raycast 변수
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //Grid 시스템
    private Grid grid;
    private bool mode_build = false;

    Vector3 truePos;
    public float gridSize;


    private void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void SlotClick(int _slotNumber)
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        tf_player = playerController.theCamera.transform;
        Object_Prefab = craft_Tab[_slotNumber].Object_prefab;
        Object_Preview = Instantiate(craft_Tab[_slotNumber].Object_prview_Prefab, tf_player.position + tf_player.forward, Quaternion.identity);
        isPreViewActivated = true;
        Inventory_UI.SetActive(false);
        slotNumber = _slotNumber;
    }


   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isPreViewActivated)
            Inventory();//Winodws

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        if (Input.GetKeyDown(KeyCode.E))
            mode_build = !mode_build;

        if (isPreViewActivated)
            previewPositionUpdate();
  
        if (Input.GetButtonDown("Fire1"))
            Build(slotNumber);
 
       
            

           
    }

    public void Build(int _slotNumber)
    {
        if(isPreViewActivated)
        {
            //Instantiate(Object_Prefab, hitInfo.point, Quaternion.identity);
            PhotonNetwork.Instantiate(craft_Tab[_slotNumber].Name, hitInfo.point, Quaternion.identity);
            Destroy(Object_Preview);
            isActivated = false;
            isPreViewActivated = false;
            Object_Prefab = null;
            Object_Preview = null;

        }
    }

    private void previewPositionUpdate()
    {
       if(!mode_build)
        {
            Ray ray = playerController.theCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform != null)
                {
                    Vector3 _location = hitInfo.point;
                    Object_Preview.transform.position = _location;
                }

            }
        }
        else
        {
            Ray ray = playerController.theCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform != null)
                {
                     Object_Preview.transform.position = grid.GetNearestPointOnGrid(hitInfo.point);
                }

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
        Object_Prefab = null;
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
