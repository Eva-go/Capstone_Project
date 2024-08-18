using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public GameObject currentPlayer { get; private set; }



    public PhotonView pv;
    public string nickName;
    public static float Hp = 100f;
    public GameObject cam;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    [SerializeField] public Camera theCamera;
    [SerializeField] private Camera toolCamera;
    private Rigidbody myRigid;
    private CapsuleCollider myCollider;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float crouchHeight = 0.5f;
    private float originalCameraY;
    private float originalToolCameraY;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private bool live;

    private Vector3 velocity;
    private Vector3 UpCenter;
    private Vector3 DownCenter;

    public AudioSource sfx_PlayerHit, sfx_PlayerJump, sfx_PlayerWalk, sfx_PlayerSwing, sfx_PlayerDrown;
    public GameObject UnderwaterPPSVolume, DeathPPSVolume;


    private RaycastHit hitInfo;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator Localanimator;

    public GameObject[] weapons;
    public Transform weaponHoldPoint;
    public GameObject[] weaponsSwitching;
    private int selectedWeaponIndex = 0;
    private int selectedWeaponStrength = 1;

    public static int getMoney;
    public GameObject insidegameObject;
    public static bool insideActive;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Wavetransform wavetransform;
    private float damageInterval = 1.0f;
    private float tickDamage = 5.0f;
    private float lastDamageTime = 0;

    private bool Jumpforgived = false;
    private bool isWallCliming = false;
    private bool isSwimming = false;
    private bool isUnderWater = false;

    private float JumpforgivenessTime = 0;

    public float speeed = 0;

    private string[] nodeName = { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap" };
    private string[] poiName = { "Poi_Distiller", "Poi_Dryer", "Poi_Filter", "Poi_Grinder", "Poi_Heater", "Poi_Smelter" };
    public int[] nodeItiems = new int[6];
    public int[] mixItiems = new int[6];
    public int[] nodeCounts = new int[6];

    public static int[] nodeSell = new int[6];
    public static int[] mixSell = new int[6];

    public event Action OnInventoryChanged;

    public Inventory PlayerInventory = new Inventory { };

    //[SerializeField] private UI_Inventory ui_Inventory;

    [SerializeField] private ScriptableObject_Item[] InitalItems;


    public float positionLerpSpeed = 8f;
    public float rotationLerpSpeed = 8f;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public bool ShopActive;

    //리스폰 관련 변수
    private static GameObject player;
    public Transform oldTransform;
    public Transform AptTransform;

    //아파트 진입변수
    private bool isInside;
    private bool isOutside;
    public int inside;
    public bool keydowns;
    public PlayerAPTPlaneSpawn PlayerAPT;
    private bool oldPosState = false; // 상태를 추적하기 위한 변수
    private bool insideState = false; // 상태를 추적하기 위한 변수
    Transform parentTransform;

    private Dictionary<string, Vector3> doorPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> doorRotations = new Dictionary<string, Quaternion>();
    private string lastDoorEntered;

    //InteractableObject를 위한 코드
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetPlayer(GameObject player)
    {
        currentPlayer = player;
    }
    //InteractableObject를 위한 코드 끝

    public void InvokeInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void EnterDoor(Transform doorTransform)
    {
        string doorName = doorTransform.name; // 문 이름을 사용하여 고유 식별자로 사용
        lastDoorEntered = doorName;

        // 문 위치와 회전 저장
        if (!doorPositions.ContainsKey(doorName))
        {
            doorPositions[doorName] = doorTransform.position;
            doorRotations[doorName] = doorTransform.rotation;
        }
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            myRigid = GetComponent<Rigidbody>();
            myCollider = GetComponent<CapsuleCollider>();
            PlayerAPT = GetComponent<PlayerAPTPlaneSpawn>();
            cam.SetActive(true);
            LocalPlayerManger.Instance.RegisterLocalPlayer(this);
            CanvasController.Instance.RegisterPlayerController(this);
            oldTransform = gameObject.transform;
            AptTransform = gameObject.transform;
            inside = 0;
            keydowns = false;

            ShopActive = false;
            live = true;
            GameValue.Money_total = 0;
            GameValue.setMoney();

            GameValue.Axe = 0;
            GameValue.Pickaxe = 0;
            GameValue.Shovel = 0;

            for (int i = 0; i < InitalItems.Length; i++)
            {
                PlayerInventory.AddItem(new Item { ItemType = InitalItems[i], Count = 1 });
            }
        }

        nickName = this.gameObject.name;

        EquipWeapon(selectedWeaponIndex);
        Cursor.lockState = CursorLockMode.Locked;

        originalCameraY = theCamera.transform.localPosition.y;
        originalToolCameraY = toolCamera.transform.localPosition.y;

        UpCenter = new Vector3(0f, 1f, 0f);
        DownCenter = new Vector3(0f, 0.5f, 0f);

        Items();
        wavetransform = FindObjectOfType<Wavetransform>();
        Hp = 100;
    }

    void OnEnable()
    {
        if (pv.IsMine)
        {
            LocalPlayerManger.Instance.RegisterLocalPlayer(this);
            CanvasController.Instance.RegisterPlayerController(this);
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            cam.SetActive(true);
            Move();
            if (!isRunning)
            {
                Crouch();
            }
            if (!isCrouching)
            {
                Run();
            }
            CameraRotation();
            CharacterRotation();
            Jump();
            Interaction();
            Attack();
            Switching();
            WaveTic();
            Sell();
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Hp = 100;
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Hp = -1;
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                GameValue.GetMomey(1000);
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                GameValue.Round = 0;
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                IncreaseLocalPlayerItems();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                for (int i = 0; i < 6; i++)
                {
                    Debug.Log("노드 아이템 " + nodeItiems[i]);
                    Debug.Log("믹스 아이템 " + mixItiems[i]);
                }
            }
            if (Hp <= 0)
            {
                Die();
                DeathPPSVolume.SetActive(true);
            }
            else if (DeathPPSVolume.activeSelf)
            {
                DeathPPSVolume.SetActive(false);
            }
            //if (insideActive && InsideFillHandler.fillValue >= 100)
            //{
            //    Debug.Log("인사이드" + inside);
            //    InsideFillHandler.fillValue = 0;
            //    InsideUpdate();
            //    keydowns = false;
            //    myRigid.isKinematic = false;
            //}
            if (GameValue.exit)
            {
                Destroy(gameObject);
            }
        }
    }

    /*public void InsideUpdate()
    {
        switch(inside)
        {
            case 0:
                gameObject.transform.position = parentTransform.position;
                gameObject.transform.rotation = Quaternion.Euler(PlayerAPT.playerrotation);
                break;
            case 1:
                gameObject.transform.position = PlayerAPT.playerPoint;
                gameObject.transform.rotation = Quaternion.Euler(PlayerAPT.playerrotation);
                break;
            case 2:
                if (doorPositions.ContainsKey(lastDoorEntered))
                {
                    gameObject.transform.position = doorPositions[lastDoorEntered]; // 마지막으로 들어갔던 문 위치로 이동
                    gameObject.transform.rotation = doorRotations[lastDoorEntered]; // 마지막으로 들어갔던 문의 회전 값으로 설정
                }
                break;

        }
    }*/

    public void Sell()
    {
        for (int i = 0; i < 6; i++)
        {
            nodeSell[i] = nodeItiems[i];
            mixSell[i] = mixItiems[i];
        }
        GameValue.getItem();
    }

    public void Items()
    {
        for (int i = 0; i < 6; i++)
        {
            nodeItiems[i] = 0;
            mixItiems[i] = 0;
        }
    }

    public void WaveTic()
    {
        float waveHeight, WaterDepth;
        waveHeight = wavetransform.GetWaveHeight(transform.position.x * 0.1f, transform.position.z * 0.1f, wavetransform.waveStrength);
        WaterDepth = waveHeight + wavetransform.waveY - transform.position.y;

        if (WaterDepth > 1.8f)
        {
            if (!UnderwaterPPSVolume.activeSelf)
            {
                UnderwaterPPSVolume.SetActive(true);
            }
        }
        else
        {
            UnderwaterPPSVolume.SetActive(false);
        }

        if (WaterDepth > 0.25f)
        {
            isUnderWater = true;
            if (!isSwimming && !isGrounded) isSwimming = true;

            if (!isCrouching) myRigid.AddForce(Vector3.up * 5 * WaterDepth, ForceMode.Impulse);
            else myRigid.AddForce(Vector3.up * 5, ForceMode.Impulse);

            if (Time.time >= lastDamageTime + damageInterval)
            {
                if (!sfx_PlayerDrown.isPlaying) sfx_PlayerDrown.Play();
                pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, pv.ViewID, tickDamage * wavetransform.waveStrength + WaterDepth * WaterDepth / 10);
                lastDamageTime = Time.time;
            }
        }
        else
        {
            if (isUnderWater) isUnderWater = false;
            if (isSwimming) isSwimming = false;
        }
    }

    private void Die()
    {
        if (pv.IsMine)
        {
            if (pv.IsMine && live)
            {
                // Bag 생성
                GameObject bag = PhotonNetwork.Instantiate("Bag", transform.position, transform.rotation, 0);
                BagController bagScript = bag.GetComponent<BagController>();
                if (bagScript != null)
                {
                    // 아이템 데이터 전송
                    for (int i = 0; i < nodeItiems.Length; i++)
                    {
                        bagScript.photonView.RPC("GetItme", RpcTarget.AllBuffered, nodeItiems[i], mixItiems[i], i);
                    }
                }

                // 아이템 초기화
                for (int i = 0; i < nodeItiems.Length; i++)
                {
                    nodeItiems[i] = 0;
                }
                InvokeInventoryChanged();

                // 게임 오브젝트 파괴
                PhotonNetwork.Destroy(gameObject);

                live = false;
                reSpwan();
            }
        }
    }
    private void reSpwan()
    {
        if (pv.IsMine && !live)
        {
            Transform parentTransform = GameObject.Find("SpawnPoint").transform;
            List<Transform> directChildren = new List<Transform>();

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                directChildren.Add(child);
            }

            if (directChildren.Count > 0)
            {
                
                int idx = UnityEngine.Random.Range(0, directChildren.Count);
                Debug.Log("스폰포인트: " + idx);
                SpawnPlayer(idx, directChildren.ToArray());
            }
            else
            {
                Debug.LogError("스폰 포인트가 없습니다.");
            }
        }
    }

    public void SpawnPlayer(int idx, Transform[] points)
    {
        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
        if (player != null)
        {
            player.name = PhotonNetwork.LocalPlayer.NickName;
            Transform OtherPlayer = player.transform.Find("OtherPlayer");
            Transform LocalPlayer = player.transform.Find("LocalPlayer");
            Transform Tool = player.transform.Find("Player001");
            Transform T_LocalPlayerTool = player.transform.Find("ToolCamera");

            if (OtherPlayer != null) OtherPlayer.gameObject.SetActive(false);
            if (LocalPlayer != null) LocalPlayer.gameObject.SetActive(true);
            if (Tool != null) Tool.gameObject.SetActive(false);
            if (T_LocalPlayerTool != null) T_LocalPlayerTool.gameObject.SetActive(true);

            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            Debug.Log("Player spawned and ownership transferred.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Player object destroyed.");
    }

    private void Move()
    {
        if (!isRunning)
        {
            animator.SetBool("isRuns", false);
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");
            if (moveDirX != 0 || moveDirZ != 0)
            {
                float movespeed = myRigid.velocity.magnitude;
                animator.SetFloat("MoveAniModifire", movespeed / walkSpeed);
                if (isGrounded && movespeed > 1) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;

                animator.SetBool("isMove", true);
                Localanimator.SetBool("isMove", true);
                Localanimator.SetFloat("MoveSpeed", 1);

                float moveDirY = myRigid.velocity.y;

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded || isUnderWater)
                {
                    velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;
                }
                else
                {
                    velocity = (moveHorizontal + moveVertical).normalized * walkSpeed * 0.5f;
                }

                velocity.y = moveDirY;
                myRigid.velocity = velocity;
            }
            else
            {
                sfx_PlayerWalk.mute = true;
                animator.SetBool("isMove", false);
                Localanimator.SetBool("isMove", false);
            }
            if (pv.IsMine)
            {
                pv.RPC("RPC_UpdatePositionAndRotation", RpcTarget.AllBuffered, transform.position, transform.rotation);
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isSwimming)
        {
            myRigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isWallCliming))
        {
            if (!sfx_PlayerJump.isPlaying)
            {
                sfx_PlayerJump.Play();
            }
            if (myRigid.velocity.y > 0)
            {
                float moveDirX = myRigid.velocity.x;
                float moveDirZ = myRigid.velocity.z;
                velocity = new Vector3(moveDirX, 0, moveDirZ);
                myRigid.velocity = velocity;
            }
            myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isGrounded = false;
            myRigid.drag = 1;
            animator.SetBool("isGrounded", false);
            Jumpforgived = false;
        }

        if (Jumpforgived)
        {
            if (Time.time >= JumpforgivenessTime + 0.25f)
            {
                isGrounded = false;
                myRigid.drag = 1;
                animator.SetBool("isGrounded", false);
                Jumpforgived = false;
                isWallCliming = false;
            }
        }
    }

    private void Run()
    {
        if (!isCrouching)
            isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isRunning && !isCrouching)
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");
            if (moveDirX != 0 || moveDirZ != 0)
            {
                float movespeed = myRigid.velocity.magnitude;
                animator.SetFloat("MoveAniModifire", movespeed / runSpeed);
                if (isGrounded && movespeed > 1) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;

                animator.SetBool("isRuns", true);
                Localanimator.SetBool("isMove", true);
                Localanimator.SetFloat("MoveSpeed", 1.5f);
                float moveDirY = myRigid.velocity.y;
                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded || isUnderWater)
                {
                    velocity = (moveHorizontal + moveVertical).normalized * runSpeed;
                }
                else
                {
                    velocity = (moveHorizontal + moveVertical).normalized * runSpeed * 0.5f;
                }

                velocity.y = moveDirY;
                myRigid.velocity = velocity;
            }
            else
            {
                sfx_PlayerWalk.mute = true;
                animator.SetBool("isRuns", false);
                Localanimator.SetBool("isMove", false);
            }
            if (pv.IsMine)
            {
                pv.RPC("RPC_UpdatePositionAndRotation", RpcTarget.AllBuffered, transform.position, transform.rotation);
            }
        }
    }

    private void Crouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching && !isRunning)
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");
            if (moveDirX != 0 || moveDirZ != 0)
            {
                float movespeed = myRigid.velocity.magnitude;
                animator.SetFloat("MoveAniModifire", movespeed / crouchSpeed);
                if (isGrounded && movespeed > 1) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;

                animator.SetBool("isCrouchWalk", true);
                Localanimator.SetBool("isMove", true);
                Localanimator.SetFloat("MoveSpeed", 0.5f);
                float moveDirY = myRigid.velocity.y;

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded || isUnderWater)
                {
                    velocity = (moveHorizontal + moveVertical).normalized * crouchSpeed;
                }
                else
                {
                    velocity = (moveHorizontal + moveVertical).normalized * crouchSpeed * 0.5f;
                }

                velocity.y = moveDirY;
                myRigid.velocity = velocity;
            }
            else
            {
                sfx_PlayerWalk.mute = true;
                animator.SetBool("isCrouchWalk", false);
                Localanimator.SetBool("isMove", false);
            }
        }

        if (isCrouching)
        {
            animator.SetBool("isCrouch", true);
            theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, originalCameraY * crouchHeight, theCamera.transform.localPosition.z);
            toolCamera.transform.localPosition = new Vector3(toolCamera.transform.localPosition.x, originalToolCameraY * crouchHeight, toolCamera.transform.localPosition.z);
        }
        else
        {
            animator.SetBool("isCrouch", false);
            theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, originalCameraY, theCamera.transform.localPosition.z);
            toolCamera.transform.localPosition = new Vector3(toolCamera.transform.localPosition.x, originalToolCameraY, toolCamera.transform.localPosition.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PLANE")
        {
            isGrounded = true;
            myRigid.drag = 5;
            animator.SetBool("isGrounded", true);
            Jumpforgived = false;
            isWallCliming = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "PLANE")
        {
            if (Jumpforgived)
            {
                isGrounded = false;
                myRigid.drag = 1;
                animator.SetBool("isGrounded", false);
                Jumpforgived = false;
            }
            else
            {
                Jumpforgived = true;
                JumpforgivenessTime = Time.time;
            }
            isWallCliming = false;
        }
    }

    private void CameraRotation()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float xRotation = Input.GetAxisRaw("Mouse Y");
            float cameraRotationX = xRotation * lookSensitivity;
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            toolCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    private void CharacterRotation()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
            myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
        }
    }

    private void Interaction()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        keydowns = Input.GetKey(KeyCode.E);

        if (keydowns && Physics.Raycast(ray, out hitInfo, 5))
        {
            if (hitInfo.collider.CompareTag("Door"))
            {
                //isInside = false;
                myRigid.isKinematic = true;
                insideActive = true;
                // 문을 통과하여 아파트로 들어가는 경우
                EnterDoor(hitInfo.collider.transform); // 문 위치를 저장
               
                if (InsideFillHandler.fillValue >= 100)
                {
                    inside = 1;
                    keydowns = false;
                    insideActive = false;
                    //InsideUpdate();
                    gameObject.transform.position = PlayerAPT.playerPoint;
                    Debug.Log("pos2" + gameObject.transform.position);
                    gameObject.transform.rotation = Quaternion.Euler(PlayerAPT.playerrotation);
                    myRigid.isKinematic = false;
                    isInside = true;
                    InsideFillHandler.fillValue = 0;
                }
                if (isInside)
                {
                    InsideFillHandler.fillValue = 0;
                    if (gameObject.transform.position != PlayerAPT.playerPoint)
                    {
                        gameObject.transform.position = PlayerAPT.playerPoint;
                        Debug.Log("pos1" + gameObject.transform.position);
                        isInside = false;
                    }
                }
               
            }
            else if (hitInfo.collider.CompareTag("ReturnDoor"))
            {
                isOutside = false;
                myRigid.isKinematic = true;
                insideActive = true;
                // 이전에 저장된 문 위치로 되돌아가는 경우
         
                if (InsideFillHandler.fillValue >= 100)
                {
                    inside = 2;
                    keydowns = false;
                    insideActive = false;
                    //InsideUpdate();    
                    gameObject.transform.position = doorPositions[lastDoorEntered]; // 마지막으로 들어갔던 문 위치로 이동
                    gameObject.transform.rotation = doorRotations[lastDoorEntered]; // 마지막으로 들어갔던 문의 회전 값으로 설정
                    myRigid.isKinematic = false;
                    isOutside = true;
                    InsideFillHandler.fillValue = 0;
                }
                if (isOutside)
                {
                    InsideFillHandler.fillValue = 0;
                    if (gameObject.transform.position != doorPositions[lastDoorEntered])
                    {
                        gameObject.transform.position = doorPositions[lastDoorEntered];
                        Debug.Log("pos1" + gameObject.transform.position);
                        isOutside = false;
                    }
                }
               
            }
            else
            {
                myRigid.isKinematic = false;
                insideActive = false;
            }
        }
        else
        {
            myRigid.isKinematic = false;
            insideActive = false;
        }


        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.tag == "Portal")
        {
            ShopActive = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.tag == "Poi")
        {
            PhotonView targetPv = hitInfo.collider.GetComponent<PhotonView>();
            if (targetPv != null)
            {
                PoiController poiController = hitInfo.collider.GetComponent<PoiController>();
                if (poiController != null)
                {
                    //stationinteration

                    for (int i=0; i<nodeItiems.Length;i++)
                    {
                        if (poiController.name.Equals(poiName[i] + "(Clone)")&& nodeItiems[i]>0)
                        {
                            nodeItiems[i]--;
                            targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[i], nodeName[i], nickName,i);
                        }
                            
                    }
                }
            }
        }
    }

    public void Attack_Time()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.CompareTag("Node"))
            {
                NodeController nodeController = hit.collider.GetComponent<NodeController>();
                if (nodeController != null)
                {
                    float CombatSwingMult = 1;

                    if (isCrouching || isRunning || !isGrounded) CombatSwingMult = 0.75f;

                    switch (selectedWeaponIndex)
                    {
                        case 0:
                            selectedWeaponStrength = GameValue.Axe + 1;
                            break;
                        case 1:
                            selectedWeaponStrength = GameValue.Pickaxe + 1;
                            break;
                        case 2:
                            selectedWeaponStrength = GameValue.Shovel + 1;
                            break;
                        default:
                            selectedWeaponStrength = 1;
                            break;
                    }

                    if (nodeController.Node_Type == selectedWeaponIndex)
                    {
                        nodeController.TakeDamage(10f * selectedWeaponStrength * CombatSwingMult, true);
                    }
                    else
                    {
                        nodeController.TakeDamage(5f * selectedWeaponStrength * CombatSwingMult, false);
                    }
                    PlayerInventory.AddItem(new Item { ItemType = nodeController.NodeItemType, Count = nodeController.nodeCount });

                    for (int i = 0; i < 6; i++)
                    {
                        if (nodeController.nodeName.Equals("node_" + nodeName[i] + "(Clone)"))
                        {
                            nodeItiems[i] += nodeController.nodeCount;
                        }
                    }

                }
            }
            else if (hit.collider.CompareTag("Player"))
            {
                if (isCrouching || isRunning || !isGrounded) pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID, 15f);
                else pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID, 10f);
            }
            else if (hit.collider.CompareTag("Bag"))
            {
                PhotonView bagPhotonView = hit.collider.GetComponent<PhotonView>();
                if (bagPhotonView != null)
                {
                    float damage = 10f;
                    bagPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage, pv.ViewID);
                }
            }
            else if (hit.collider.CompareTag("Poi"))
            {
                PoiController poiController = hit.collider.GetComponent<PoiController>();
                if(poiController != null)
                {
                    for(int i=0;i<6;i++)
                    {
                        if (poiController.poiName.Equals(poiName[i] + "(Clone)"))
                        {
                            Debug.Log("에러" + poiController.name); ;
                            int mixItemCount = poiController.itemData.mixItemCount[i];
                            pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                        }
                            
                    }
                  
                }
            }
        }
    }

    private void Attack()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetMouseButton(0))
            {
                if (!sfx_PlayerSwing.isPlaying)
                {
                    sfx_PlayerSwing.Play();

                    if (isCrouching || isRunning || !isGrounded)
                    {
                        animator.SetTrigger("Combat_Swing");
                        Localanimator.SetTrigger("Combat_Swing");
                    }
                    else
                    {
                        animator.SetTrigger("Swing");
                        Localanimator.SetTrigger("Swing");
                    }
                }
            }
        }
    }

    [PunRPC]
    private void RPC_UpdatePositionAndRotation(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
    }

    [PunRPC]
    private void RPC_TakeDamage(int viewID, float damage)
    {
        PhotonView targetPv = PhotonView.Find(viewID);
        if (targetPv != null)
        {
            PlayerController targetPlayer = targetPv.GetComponent<PlayerController>();
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!sfx_PlayerHit.isPlaying)
        {
            sfx_PlayerHit.Play();
        }
        if (pv.IsMine)
        {
            animator.SetTrigger("Hit");
            Hp -= damage;
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["HP"] = Hp;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

            if (Hp <= 0)
            {
                Die();
            }
        }
    }

    private void Switching()
    {
        int previousSelectedWeaponIndex = selectedWeaponIndex;
        if (GameValue.Axe == 1)
        {
            weapons[0] = weaponsSwitching[0];


        }
        else if (GameValue.Axe == 2)
        {
            weapons[0] = weaponsSwitching[3];

        }
        if (GameValue.Pickaxe == 1)
        {
            weapons[1] = weaponsSwitching[1];

        }
        else if (GameValue.Pickaxe == 2)
        {
            weapons[1] = weaponsSwitching[4];

        }
        if (GameValue.Shovel == 1)
        {
            weapons[2] = weaponsSwitching[2];

        }
        else if (GameValue.Shovel == 2)
        {
            weapons[2] = weaponsSwitching[5];

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length >= 2)
        {
            selectedWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length >= 3)
        {
            selectedWeaponIndex = 2;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex + 1) % 3;
        }
        else if (scroll < 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex + 2) % 3;
        }

        if (previousSelectedWeaponIndex != selectedWeaponIndex)
        {
            pv.RPC("RPC_EquipWeapon", RpcTarget.AllBuffered, selectedWeaponIndex);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    [PunRPC]
    private void RPC_EquipWeapon(int index)
    {
        EquipWeapon(index);
    }

    [PunRPC]
    public void UpdateMixItem(int index, int mixItemCount)
    {
        mixItiems[index] = mixItemCount;
    }

    public void UpdateNodeItem(int index, int newCount)
    {
        if (pv.IsMine)
        {
            if (index >= 0 && index < nodeItiems.Length)
            {
                nodeItiems[index] = newCount;
                OnInventoryChanged?.Invoke();
            }
        }
    }

    private void IncreaseLocalPlayerItems()
    {
        if (pv.IsMine)
        {
            for (int i = 0; i < nodeItiems.Length; i++)
            {
                nodeItiems[i] += 10;
              
            }
            OnInventoryChanged?.Invoke();
        }
    }

    private void EquipWeapon(int index)
    {
        foreach (Transform child in weaponHoldPoint)
        {
            Destroy(child.gameObject);
        }

        GameObject weaponInstance = Instantiate(weapons[index], weaponHoldPoint.position, weaponHoldPoint.rotation);
        weaponInstance.transform.SetParent(weaponHoldPoint);
    }
}