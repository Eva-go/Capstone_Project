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

    public GameObject StationList;
    public GameObject StationInfoTab;
    public GameObject BuildButton;

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

    public float MaxHealth = 10;
    public float ProcessEfficiency = 1;
    public float TempertureLimit = 100;

    public ScriptableObject_Station[] SelectableStations; // 스테이션 스크립트오브젝트
    public ScriptableObject_Station SelectedStation;

    public ScriptableObject_Item[] StationMaterial = new ScriptableObject_Item[5];
    public ScriptableObject_Item AuxMat;
    public ScriptableObject_Item FixMat;


    public Sprite NullSprite;
    public Sprite EmptySprite;
    public Sprite DisabledSprite;


    public int Rotation;



    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            playerController = GetComponent<PlayerController>(); // 플레이어 컨트롤러 가져오기
            StationList = GameObject.FindWithTag("PoiList").transform.Find("PoiList").gameObject;
            canvas = GameObject.FindWithTag("Canvas").transform.Find("CanvasController").gameObject;
            canvasController = canvas.GetComponent<CanvasController>();

            StationInfoTab = StationList.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;

            for (int i = 0; i < 7; i++)
            {
                Button_ConMat buttonItem;
                buttonItem = StationInfoTab.transform.GetChild(3 + i).gameObject.GetComponent<Button_ConMat>();
                buttonItem.Station_Construction_Master = gameObject.GetComponent<PlayerPoiSpawn>();
            }

            BuildButton = StationList.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
            Button build_BT = BuildButton.GetComponent<Button>();
            build_BT.onClick.AddListener(() => BuildButtonPressed());

            for (int i = 0; i < PoiTab.Length; i++)
            {
                PoiTab[i] = StationList.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.transform.GetChild(i).gameObject; // PoiTab 배열 초기화
                Poi_BT[i] = PoiTab[i].GetComponent<Button>(); // Poi_BT 배열 초기화
                int index = i; // 인덱스를 로컬 변수로 복사
                Poi_BT[i].onClick.AddListener(() => SlotClick(index)); // 버튼 클릭 리스너 추가

                Image image = PoiTab[i].transform.GetChild(1).GetChild(0).GetComponent<Image>();
                image.sprite = SelectableStations[i].StationIcon;

                Text text = PoiTab[i].transform.GetChild(2).GetComponent<Text>();
                text.text = SelectableStations[i].Name;

                text = PoiTab[i].transform.GetChild(3).GetComponent<Text>();
                text.text = SelectableStations[i].Description;
            }
            ApplySelectedStationInfo();
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
                    Rotation = (Rotation + 1) % 4;
                    RotatePreview(); // Rotate 90 degrees clockwise
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    Rotation = (Rotation + 3) % 4;
                    RotatePreview(); // Rotate 90 degrees counterclockwise
                }
            }

            if (Input.GetButtonDown("Fire1") && isPreViewActivated && !isColliding)
            {
                Build(slotNumber); // Build the object at the preview position
            }
            if (Input.GetButtonDown("Fire2"))
            {
                Cancel();

            }
        }
    }

    public void Cancel()
    {
        if (previewObjectInstance != null)
        {
            Destroy(previewObjectInstance); // Destroy the preview object
        }
        isPreViewActivated = false; // Deactivate preview
        previewObjectInstance = null;
    }

    public void Build(int _slotNumber)
    {
        if (isPreViewActivated && _slotNumber >= 0 && _slotNumber < SpawnPoi.Length)
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
                GameObject station = PhotonNetwork.Instantiate(SpawnPoi[_slotNumber].name, roundedPosition, previewObjectInstance.transform.rotation);

                StationMatController stationMatController = station.GetComponent<StationMatController>();

                if (stationMatController != null)
                {
                    stationMatController.StationAuxMat = AuxMat;
                    stationMatController.StationFixMat = FixMat;
                    for (int i = 0; i < stationMatController.StationConMat.Length; i++)
                    {
                        stationMatController.StationConMat[i] = StationMaterial[i];
                    }
                    stationMatController.UpdateMatStation();
                }

                PoiController stationController = station.GetComponent<PoiController>();

                if (stationController != null)
                {
                    stationController.MaxHealth = MaxHealth;
                    stationController.ProcessEfficiency = ProcessEfficiency;
                    stationController.TempertureLimit = TempertureLimit;
                }

                foreach (Item item in playerController.ConstInventory.GetItems())
                {
                    if (stationMatController != null) stationMatController.StationConstInv.AddItem(item);
                    playerController.PlayerInventory.RemoveItem(item);
                }
                playerController.ConstInventory.ClearInventory();
                playerController.InvokeInventoryChanged();


                if (previewObjectInstance != null)
                {
                    Destroy(previewObjectInstance); // Destroy the preview object
                }
                isPreViewActivated = false; // Deactivate preview
                previewObjectInstance = null;
            }
        }
        else
        {
            
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
                RotatePreview();
            }

            // Check for collision and update preview object
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Poi")||hitInfo.collider.CompareTag("Pipe"))
                {
                    if (!isColliding)
                    {
                        isColliding = true;
                        Destroy(previewObjectInstance); // Destroy current preview object
                        previewObjectInstance = Instantiate(PreviewPoi_red[slotNumber], _location, Quaternion.identity);
                        RotatePreview();
                    }
                }
                else
                {
                    if (isColliding)
                    {
                        isColliding = false;
                        Destroy(previewObjectInstance); // Destroy current preview object
                        previewObjectInstance = Instantiate(PreviewPoi_green[slotNumber], _location, Quaternion.identity);
                        RotatePreview();
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
                    RotatePreview();
                }
            }
        }
        else
        {
            Vector3 _location = playerController.theCamera.transform.position + playerController.theCamera.transform.forward * 4f;
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
                previewObjectInstance = Instantiate(PreviewPoi_red[slotNumber], _location, Quaternion.identity);
                RotatePreview();
            }
            if (!isColliding)
            {
                isColliding = true;
                Destroy(previewObjectInstance); // Destroy current preview object
                previewObjectInstance = Instantiate(PreviewPoi_red[slotNumber], _location, Quaternion.identity);
                RotatePreview();
            }
        }
    }

    private void RotatePreview()
    {
        if (previewObjectInstance != null)
        {
            previewObjectInstance.transform.eulerAngles = new Vector3
            {
                x = previewObjectInstance.transform.eulerAngles.x,
                y = Rotation * 90f,
                z = previewObjectInstance.transform.eulerAngles.z
            };
            //previewObjectInstance.transform.Rotate(Vector3.up, Rotation * 90f); // Rotate the preview object around the Y-axis

        }
    }

    public void BuildButtonPressed()
    {
        if (BuildableTest())
        {
            onClickStart();
        }
    }

    public bool BuildableTest()
    {
        bool isBuildable = false;
        if (SelectedStation != null)
        {
            if (
                (!SelectedStation.StationAux || AuxMat != null) &&
                (!SelectedStation.StationFix || FixMat != null)
                )
            {
                bool itemSelected = true;
                for (int i = 0; i < SelectedStation.StationMaterialCount; i++)
                {
                    if (StationMaterial[i] == null)
                    {
                        itemSelected = false;
                    }
                }
                if (itemSelected)
                {
                    isBuildable = true;
                }
            }
        }
        Button button = BuildButton.GetComponent<Button>();
        button.interactable = isBuildable;
        return isBuildable;
    }
    
    public void SlotClick(int _slotNumber)
    {
        slotNumber = _slotNumber; // Set the slot number

        if (slotNumber >= 0 && slotNumber < SelectableStations.Length)
        {
            SelectedStation = SelectableStations[slotNumber];
            if (SelectedStation != null) StationValueCaculate();
        }
        ClearConMatItem();
        ApplySelectedStationInfo();
        //onClickStart(); // Call the start method for preview
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

    public void GetConMatItem()
    {
        playerController.ConstInventory.ClearInventory();

        if (SelectedStation != null)
        {
            if (StationList != null)
            {
                Button_ConMat buttonItem = StationInfoTab.transform.GetChild(3).GetComponent<Button_ConMat>();
                if (SelectedStation.StationAux && buttonItem.SelectedItem != null)
                {
                    AuxMat = buttonItem.SelectedItem;
                    playerController.ConstInventory.AddItem(new Item { ItemType = AuxMat, Count = 1 });
                }

                buttonItem = StationInfoTab.transform.GetChild(4).GetComponent<Button_ConMat>();
                if (SelectedStation.StationFix && buttonItem.SelectedItem != null)
                {
                    FixMat = buttonItem.SelectedItem;
                    playerController.ConstInventory.AddItem(new Item { ItemType = FixMat, Count = 1 });
                }

                for (int i = 0; i < 5; i++)
                {
                    buttonItem = StationInfoTab.transform.GetChild(5 + i).GetComponent<Button_ConMat>();
                    if (i < SelectedStation.StationMaterialCount && buttonItem.SelectedItem != null)
                    {
                        StationMaterial[i] = buttonItem.SelectedItem;
                        playerController.ConstInventory.AddItem(new Item { ItemType = StationMaterial[i], Count = 1 });
                    }
                }
                StationValueCaculate();
            }
        }
        playerController.InvokeInventoryChanged();
    }

    public void ClearConMatItem()
    {
        AuxMat = null;
        FixMat = null;
        for (int i = 0; i < StationMaterial.Length; i++)
        {
            StationMaterial[i] = null;
        }

        if (SelectedStation != null)
        {
            if (StationList != null)
            {
                Button_ConMat buttonItem = StationInfoTab.transform.GetChild(3).GetComponent<Button_ConMat>();
                Image image = StationInfoTab.transform.GetChild(3).GetChild(0).GetComponent<Image>();
                buttonItem.SelectedItem = null;
                image.sprite = NullSprite;

                buttonItem = StationInfoTab.transform.GetChild(4).GetComponent<Button_ConMat>();
                image = StationInfoTab.transform.GetChild(4).GetChild(0).GetComponent<Image>();
                buttonItem.SelectedItem = null;
                image.sprite = NullSprite;

                for (int i = 0; i < 5; i++)
                {
                    buttonItem = StationInfoTab.transform.GetChild(5 + i).GetComponent<Button_ConMat>();
                    image = StationInfoTab.transform.GetChild(5 + i).GetChild(0).GetComponent<Image>();
                    buttonItem.SelectedItem = null;
                    image.sprite = NullSprite;
                }
                StationValueCaculate();
            }
        }

        BtnConMat_CloseAllTab();
        StationInfoTab.transform.GetChild(3).gameObject.GetComponent<Button_ConMat>().ItemTab.SetActive(false);

    }

    public void ApplySelectedStationInfo()
    {
        if (SelectedStation != null)
        {
            if (StationList != null)
            {
                Text text;
                Image image;
                Button button;
                button = StationInfoTab.transform.GetChild(3).GetComponent<Button>();
                text = StationInfoTab.transform.GetChild(3).GetChild(1).GetComponent<Text>();
                image = StationInfoTab.transform.GetChild(3).GetChild(0).GetComponent<Image>();
                button.interactable = SelectedStation.StationAux;
                if (!SelectedStation.StationAux)
                {
                    text.text = "";
                    image.sprite = DisabledSprite;
                }
                else
                {
                    text.text = "Aux";
                    image.sprite = EmptySprite;
                }

                button = StationInfoTab.transform.GetChild(4).GetComponent<Button>();
                text = StationInfoTab.transform.GetChild(4).GetChild(1).GetComponent<Text>();
                image = StationInfoTab.transform.GetChild(4).GetChild(0).GetComponent<Image>();
                button.interactable = SelectedStation.StationFix;
                if (!SelectedStation.StationFix)
                {
                    text.text = "";
                    image.sprite = DisabledSprite;
                }
                else
                {
                    text.text = "Fix";
                    image.sprite = EmptySprite;
                }

                for (int i = 0; i < 5; i++)
                {
                    button = StationInfoTab.transform.GetChild(5 + i).GetComponent<Button>();
                    text = StationInfoTab.transform.GetChild(5 + i).GetChild(1).GetComponent<Text>();
                    image = StationInfoTab.transform.GetChild(5 + i).GetChild(0).GetComponent<Image>();
                    if (i < SelectedStation.StationMaterialCount)
                    {
                        button.interactable = true;
                        switch (SelectedStation.StationMatType[i])
                        {
                            case 1:
                                text.text = "In";
                                break;
                            case 2:
                                text.text = "Out";
                                break;
                            case 3:
                                text.text = "InOut";
                                break;
                            case 4:
                                text.text = "Fuel";
                                break;
                            case 5:
                                text.text = "Cool";
                                break;
                            default:
                                text.text = "";
                                break;
                        }
                        if (SelectedStation.TempertureSensitive[i]) text.color = new Color { r = 1f, g = 0f, b = 0f, a = 1f };
                        else text.color = new Color { r = 1f, g = 1f, b = 1f, a = 1f };
                        image.sprite = EmptySprite;
                    }
                    else
                    {
                        button.interactable = false;
                        text.text = "";
                        image.sprite = DisabledSprite;
                    }
                }
            }

            if (BuildButton != null)
            {
                Image image = BuildButton.transform.GetChild(0).GetComponent<Image>();
                image.sprite = SelectedStation.StationIcon;
            }
        }
        else
        {
            if (StationList != null)
            {
                Text text;
                Image image;
                Button button;
                button = StationInfoTab.transform.GetChild(3).GetComponent<Button>();
                text = StationInfoTab.transform.GetChild(3).GetChild(1).GetComponent<Text>();
                image = StationInfoTab.transform.GetChild(3).GetChild(0).GetComponent<Image>();
                button.interactable = false;
                text.text = "";
                image.sprite = DisabledSprite;

                button = StationInfoTab.transform.GetChild(4).GetComponent<Button>();
                text = StationInfoTab.transform.GetChild(4).GetChild(1).GetComponent<Text>();
                image = StationInfoTab.transform.GetChild(4).GetChild(0).GetComponent<Image>();
                button.interactable = false;
                text.text = "";
                image.sprite = DisabledSprite;

                for (int i = 0; i < 5; i++)
                {
                    button = StationInfoTab.transform.GetChild(5 + i).GetComponent<Button>();
                    text = StationInfoTab.transform.GetChild(5 + i).GetChild(1).GetComponent<Text>();
                    image = StationInfoTab.transform.GetChild(5 + i).GetChild(0).GetComponent<Image>();
                    button.interactable = false;
                    text.text = "";
                    image.sprite = DisabledSprite;
                }
            }

            if (BuildButton != null)
            {
                Image image = BuildButton.transform.GetChild(0).GetComponent<Image>();
                image.sprite = NullSprite;
            }
        }
    }

    public void StationInfoUpdate()
    {
        Text text = StationInfoTab.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = MaxHealth.ToString("F0");
        text = StationInfoTab.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        text.text = (ProcessEfficiency * 100f).ToString("F0") + "%";
        text = StationInfoTab.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        text.text = TempertureLimit.ToString("F0");
    }

    public void StationValueCaculate()
    {
        BuildableTest();
        MaxHealth = 10;
        TempertureLimit = 100;
        ProcessEfficiency = 1;

        bool TempApplied = false;
        for (int i = 0; i < SelectedStation.StationMaterialCount; i++)
        {
            if (StationMaterial[i] != null)
            {
                MaxHealth += StationMaterial[i].HealthStrength;
                ProcessEfficiency *= StationMaterial[i].ProcessEfficiency;

                if (SelectedStation.TempertureSensitive[i])
                {
                    if (!TempApplied) TempertureLimit = StationMaterial[i].TempertureLimit;
                    else if (TempertureLimit > StationMaterial[i].TempertureLimit)
                        TempertureLimit = StationMaterial[i].TempertureLimit;
                }
            }
        }

        if (SelectedStation.StationAux && AuxMat != null)
        {
            MaxHealth += AuxMat.HealthStrength;
            ProcessEfficiency *= AuxMat.ProcessEfficiency;
        }
        if (SelectedStation.StationFix && FixMat != null)
        {
            MaxHealth += FixMat.HealthStrength;
            ProcessEfficiency *= FixMat.ProcessEfficiency;
        }
        StationInfoUpdate();
    }

    public void BtnConMat_CloseAllTab()
    {
        for (int i = 0; i < 7; i++)
        {
            StationInfoTab.transform.GetChild(3 + i).gameObject.GetComponent<Button_ConMat>().Opened = false;
        }
    }
}