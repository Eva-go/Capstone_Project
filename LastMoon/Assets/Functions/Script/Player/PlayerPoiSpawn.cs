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
    public PhotonView pv;

    // 오브젝트 관련 변수
    public GameObject canvas;
    public CanvasController canvasController;
    public GameObject[] PreviewPoi_green; // 프리뷰 포인트 배열
    public GameObject[] PreviewPoi_red;
    public GameObject[] SpawnPoi; // 생성 포인트 배열
    private GameObject previewObjectInstance; // 프리뷰 오브젝트 인스턴스
    private int slotNumber; // 현재 슬롯 번호

    // 상호작용 변수
    private bool isPreViewActivated = false;

    // Raycast 변수
    private RaycastHit hitInfo;

    // 충돌 여부 변수
    private bool isColliding = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            playerController = GetComponent<PlayerController>(); // 플레이어 컨트롤러 가져오기
            GameObject poiLists = GameObject.FindWithTag("PoiList").transform.Find("PoiListBG").gameObject;
            canvas = GameObject.FindWithTag("Canvas").transform.Find("CanvasController").gameObject;
            canvasController = canvas.GetComponent<CanvasController>();

            for (int i = 0; i < PoiTab.Length; i++)
            {
                PoiTab[i] = poiLists.transform.GetChild(i).gameObject; // PoiTab 배열 초기화
                Poi_BT[i] = PoiTab[i].GetComponent<Button>(); // Poi_BT 배열 초기화
                int index = i; // 인덱스를 로컬 변수로 복사
                Poi_BT[i].onClick.AddListener(() => SlotClick(index)); // 버튼 클릭 리스너 추가
            }
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (isPreViewActivated)
            {
                previewPositionUpdate(); // Update preview position

                // Rotate the preview object
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RotatePreview(90f); // Rotate 90 degrees clockwise
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotatePreview(-90f); // Rotate 90 degrees counterclockwise
                }
            }

            if (Input.GetButtonDown("Fire1") && isPreViewActivated && !isColliding)
            {
                Build(slotNumber); // Build the object at the preview position
            }
        }
    }

    public void Build(int _slotNumber)
    {
        if (isPreViewActivated)
        {
            // Ensure hitInfo.point is valid
            if (hitInfo.collider != null)
            {
                Vector3 roundedPosition = new Vector3(
                    Mathf.Round(hitInfo.point.x),
                    Mathf.Round(hitInfo.point.y),
                    Mathf.Round(hitInfo.point.z)
                );
                // Instantiate the object using Photon Network for actual objects
                PhotonNetwork.Instantiate(SpawnPoi[_slotNumber].name, roundedPosition, previewObjectInstance.transform.rotation);
                if (previewObjectInstance != null)
                {
                    Destroy(previewObjectInstance); // Destroy the preview object
                }
                isPreViewActivated = false; // Deactivate preview
                previewObjectInstance = null;
            }
        }
    }

    private void previewPositionUpdate()
    {
        Ray ray = playerController.theCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 5f)) // Set max distance for raycast
        {
            Vector3 _location = hitInfo.point;
            // Round x, y, z positions to the nearest integer
            _location.x = Mathf.Round(_location.x);
            _location.y = Mathf.Round(_location.y);
            _location.z = Mathf.Round(_location.z);

            if (previewObjectInstance != null)
            {
                // Save current rotation
                Quaternion currentRotation = previewObjectInstance.transform.rotation;

                // Update position
                previewObjectInstance.transform.position = _location;

                // Restore rotation
                previewObjectInstance.transform.rotation = currentRotation;
            }
            else
            {
                // Instantiate a new preview object if none exists
                previewObjectInstance = Instantiate(PreviewPoi_green[slotNumber], _location, Quaternion.identity);
            }

            // Check for collision and update preview object
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Poi"))
                {
                    if (!isColliding)
                    {
                        isColliding = true;
                        Destroy(previewObjectInstance); // Destroy current preview object
                        previewObjectInstance = Instantiate(PreviewPoi_red[slotNumber], _location, Quaternion.identity);
                    }
                }
                else
                {
                    if (isColliding)
                    {
                        isColliding = false;
                        Destroy(previewObjectInstance); // Destroy current preview object
                        previewObjectInstance = Instantiate(PreviewPoi_green[slotNumber], _location, Quaternion.identity);
                    }
                }
            }
            else
            {
                if (isColliding)
                {
                    isColliding = false;
                    Destroy(previewObjectInstance); // Destroy current preview object
                    previewObjectInstance = Instantiate(PreviewPoi_green[slotNumber], _location, Quaternion.identity);
                }
            }
        }
    }

    private void RotatePreview(float angle)
    {
        if (previewObjectInstance != null)
        {
            previewObjectInstance.transform.Rotate(Vector3.up, angle); // Rotate the preview object around the Y-axis
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
        // Ensure only the local player sees their preview object
        if (previewObjectInstance != null)
        {
            Destroy(previewObjectInstance); // Ensure previous preview object is destroyed
        }
        // Instantiate the preview object locally
        previewObjectInstance = Instantiate(PreviewPoi_green[slotNumber], tf_player.position + tf_player.forward * 5f, Quaternion.identity);
        isPreViewActivated = true; // Activate preview
        isColliding = false; // Reset collision status
        canvasController.SetPoi = false;
        canvasController.Poi.SetActive(canvasController.SetPoi);
        Cursor.lockState = CursorLockMode.Locked;
    }
}