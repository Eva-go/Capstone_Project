using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPoiSpawn : MonoBehaviour
{
    // UI 관련 변수
    public GameObject[] PoiTab; // 포인트 탭 게임 오브젝트 배열
    public Button[] Poi_BT; // 버튼 배열

    // 플레이어 관련 변수
    private PlayerController playerController; // 플레이어 컨트롤러
    private Transform tf_player; // 플레이어 카메라 트랜스폼

    // 오브젝트 관련 변수
    public GameObject canvas;
    public CanvasController canvasController;
    public GameObject[] PreviewPoi; // 프리뷰 포인트 배열
    public GameObject[] SpawnPoi; // 생성 포인트 배열
    private GameObject previewObjectInstance; // 프리뷰 오브젝트 인스턴스
    private int slotNumber; // 현재 슬롯 번호

    // 상호작용 변수
    private bool isPreViewActivated = false;

    // Raycast 변수
    private RaycastHit hitInfo;

    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>(); // 플레이어 컨트롤러 가져오기
        GameObject poiLists = GameObject.FindWithTag("PoiList").transform.Find("PoiListBG").gameObject;
        canvas = GameObject.FindWithTag("Canvas").transform.Find("CanvasController").gameObject;
        canvasController = canvas.gameObject.GetComponent<CanvasController>();

        for (int i = 0; i < PoiTab.Length; i++)
        {
            PoiTab[i] = poiLists.transform.GetChild(i).gameObject; // PoiTab 배열 초기화
            Poi_BT[i] = PoiTab[i].GetComponent<Button>(); // Poi_BT 배열 초기화
            int index = i; // 인덱스를 로컬 변수로 복사
            Poi_BT[i].onClick.AddListener(() => SlotClick(index)); // 버튼 클릭 리스너 추가
        }
    }

    void Update()
    {
        if (isPreViewActivated)
        {
            previewPositionUpdate(); // Update preview position
        }

        if (Input.GetButtonDown("Fire1") && isPreViewActivated)
        {
            Build(slotNumber); // Build the object at the preview position
        }
    }

    public void Build(int _slotNumber)
    {
        if (isPreViewActivated)
        {
            PhotonNetwork.Instantiate(SpawnPoi[_slotNumber].name, hitInfo.point, Quaternion.identity);
            if (previewObjectInstance != null)
            {
                Destroy(previewObjectInstance); // Destroy the preview object
            }
            isPreViewActivated = false; // Deactivate preview
            previewObjectInstance = null;
        }
    }

    private void previewPositionUpdate()
    {
        Ray ray = playerController.theCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 5f)) // Set max distance for raycast
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                if (previewObjectInstance != null)
                {
                    previewObjectInstance.transform.position = _location; // Move preview object to raycast hit point
                }
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        slotNumber = _slotNumber; // Set the slot number
        onClickStart(); // Call the start method for preview
    }

    public void onClickStart()
    {
        tf_player = playerController.theCamera.transform; // Get the player camera transform
        previewObjectInstance = Instantiate(PreviewPoi[slotNumber], tf_player.position + tf_player.forward * 5f, Quaternion.identity); // Instantiate the preview object
        isPreViewActivated = true; // Activate preview
        canvasController.SetPoi = false;
        canvasController.Poi.SetActive(canvasController.SetPoi);
        Cursor.lockState = CursorLockMode.Locked;
    }
}