using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviour
{
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
    // Jump and Crouch
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

    // ray
    private RaycastHit hitInfo;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator Localanimator; // 로컬 플레이어 무기 애니메이션

    public GameObject[] weapons; // 무기 오브젝트 배열
    public Transform weaponHoldPoint; // 무기를 장착할 손 위치
    public GameObject[] weaponsSwitching;
    private int selectedWeaponIndex = 0;
    private int selectedWeaponStrength = 1;

    public static int getMoney;
    public GameObject insidegameObject;
    public static bool insideActive;
    public static bool PreViewCam;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    //파도 y축 위치
    private Wavetransform wavetransform;
    private float damageInterval = 1.0f; // interval between damage ticks
    private float tickDamage = 5.0f;     // damage per tick
    private float lastDamageTime = 0;    // time of the last tick damage

    private bool Jumpforgived = false;
    private bool isWallCliming = false;
    private bool isSwimming = false;
    private bool isUnderWater = false;

    private float JumpforgivenessTime = 0;

    public float speeed = 0;

    //인벤토리 아이템 갯수
    private string[] nodeName = { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap" };
    public int[] nodeItiems = new int[6];
    public int[] mixItiems = new int[6];
    public int[] nodeCounts = new int[6];

    //돈 치환 변수
    public static int[] nodeSell = new int[6];
    public static int[] mixSell = new int[6];

    //이벤트 처리
    public event Action OnInventoryChanged;

    //멀티플레이 보간
    public float positionLerpSpeed = 8f; // 위치 보간 속도
    public float rotationLerpSpeed = 8f; // 회전 보간 속도

    private Vector3 networkPosition;
    private Quaternion networkRotation;


    void Start()
    {
        //포톤 네트워크 연결
        pv = GetComponent<PhotonView>();
        //로컬플레이어 설정 및 로컬캔버스 설정
        LocalPlayerManger.Instance.RegisterLocalPlayer(this);
        CanvasController.Instance.RegisterPlayerController(this);

        //컴포넌트 설정
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();

        if(pv.IsMine)
        {
            cam.SetActive(true);
        }

        //플레이어 이름
        nickName = this.gameObject.name;


        // 초기 무기 장착
        EquipWeapon(selectedWeaponIndex);
        Cursor.lockState = CursorLockMode.Locked;

        PreViewCam = false;
        GameValue.setMoney();

        // 카메라의 초기 y축 위치 저장
        originalCameraY = theCamera.transform.localPosition.y;
        originalToolCameraY = toolCamera.transform.localPosition.y;

        //콜라이더 크기 조절
        UpCenter = new Vector3(0f, 1f, 0f);
        DownCenter = new Vector3(0f, 0.5f, 0f);

        //아이템 초기화
        Items();
        //파도 찾기
        wavetransform = FindObjectOfType<Wavetransform>();
        live = true;

        //돈 활성화
        GameValue.setMoney();
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
            if (Input.GetKey("escape"))
                Application.Quit();
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Hp = 100;
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                GameValue.GetMomey(1000);
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Hp = 0;
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
            // Check if the player is dead
            if (Hp <= 0)
            {
                Die();
            }
        }
        else
        {
            // 다른 플레이어의 위치와 회전을 보간하여 부드럽게 처리
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * positionLerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

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

        //if (WaterDepth > 1.8f) RenderSettings.fog = true;
        //else RenderSettings.fog = false;

        if (WaterDepth > 0.25f)
        {
            isUnderWater = true;
            if (!isGrounded) isSwimming = true;
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
            Debug.Log("Player died, starting respawn process.");
            if (live)
            {
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
            Transform[] spawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
            RespawnManager.Instance.RespawnPlayer(spawnPoints);
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
                // 네트워크 위치와 회전을 업데이트
                pv.RPC("RPC_UpdatePositionAndRotation", RpcTarget.AllBuffered, transform.position, transform.rotation);
            }
        }
    }

    private void Jump()
    {

        if (Input.GetKey(KeyCode.Space) && isSwimming)
        {
            myRigid.AddForce(Vector3.up, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isWallCliming || isSwimming))
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



        myRigid.AddForce(Vector3.up * 5, ForceMode.Impulse);

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
                // 네트워크 위치와 회전을 업데이트
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
            //myCollider.height = 1;
            //myCollider.center = DownCenter;
            animator.SetBool("isCrouch", true);
            // 카메라의 y축 위치를 crouchHeight 만큼 낮춤
            theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, originalCameraY * crouchHeight, theCamera.transform.localPosition.z);
            toolCamera.transform.localPosition = new Vector3(toolCamera.transform.localPosition.x, originalToolCameraY * crouchHeight, toolCamera.transform.localPosition.z);
        }
        else
        {
            //myCollider.height = 2;
            //myCollider.center = UpCenter;
            animator.SetBool("isCrouch", false);
            // 카메라의 y축 위치를 원래대로 되돌림
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
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        toolCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void Interaction()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKey(KeyCode.E) && Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.tag == "APT")
        {
            insideActive = true;
        }
        else
        {
            insideActive = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.tag == "Poi")
        {
            PhotonView targetPv = hitInfo.collider.GetComponent<PhotonView>();
            if (targetPv != null)
            {
                if (Physics.Raycast(ray, out hitInfo, 5f) && hitInfo.collider.CompareTag("Poi"))
                {
                    if (hitInfo.transform.gameObject.name == "Poi_Distiller(Clone)")
                    {
                        Poi_DistillerController distillerController = hitInfo.collider.GetComponent<Poi_DistillerController>();
                        if (distillerController != null)
                        {
                            int nodeIndex = distillerController.nodeNumber;
                            if (nodeItiems[nodeIndex] > 0)
                            {
                                nodeItiems[nodeIndex]--;
                                targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                            }
                        }
                    }
                    else if (hitInfo.transform.gameObject.name == "Poi_Dryer(Clone)")
                    {
                        Poi_DryerController distillerController = hitInfo.collider.GetComponent<Poi_DryerController>();
                        if (distillerController != null)
                        {
                            if (distillerController != null)
                            {
                                int nodeIndex = distillerController.nodeNumber;
                                if (nodeItiems[nodeIndex] > 0)
                                {
                                    nodeItiems[nodeIndex]--;
                                    targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                                }
                            }
                        }
                    }
                    else if (hitInfo.transform.gameObject.name == "Poi_Filter(Clone)")
                    {
                        Poi_FilterController distillerController = hitInfo.collider.GetComponent<Poi_FilterController>();
                        if (distillerController != null)
                        {
                            if (distillerController != null)
                            {
                                int nodeIndex = distillerController.nodeNumber;
                                if (nodeItiems[nodeIndex] > 0)
                                {
                                    nodeItiems[nodeIndex]--;
                                    targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                                }
                            }
                        }
                    }
                    else if (hitInfo.transform.gameObject.name == "Poi_Grinder(Clone)")
                    {
                        Poi_GrinderController distillerController = hitInfo.collider.GetComponent<Poi_GrinderController>();
                        if (distillerController != null)
                        {
                            if (distillerController != null)
                            {
                                int nodeIndex = distillerController.nodeNumber;
                                if (nodeItiems[nodeIndex] > 0)
                                {
                                    nodeItiems[nodeIndex]--;
                                    targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                                }
                            }
                        }
                    }
                    else if (hitInfo.transform.gameObject.name == "Poi_Heater(Clone)")
                    {
                        Poi_HeaterController distillerController = hitInfo.collider.GetComponent<Poi_HeaterController>();
                        if (distillerController != null)
                        {
                            if (distillerController != null)
                            {
                                int nodeIndex = distillerController.nodeNumber;
                                if (nodeItiems[nodeIndex] > 0)
                                {
                                    nodeItiems[nodeIndex]--;
                                    targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                                }
                            }
                        }
                    }
                    else if (hitInfo.transform.gameObject.name == "Poi_Smelter(Clone)")
                    {
                        Poi_SmelterController distillerController = hitInfo.collider.GetComponent<Poi_SmelterController>();
                        if (distillerController != null)
                        {
                            if (distillerController != null)
                            {
                                int nodeIndex = distillerController.nodeNumber;
                                if (nodeItiems[nodeIndex] > 0)
                                {
                                    nodeItiems[nodeIndex]--;
                                    targetPv.RPC("ReceiveData", RpcTarget.AllBuffered, nodeItiems[nodeIndex], nodeName[nodeIndex], nickName);
                                }
                            }
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
                // 플레이어 공격 처리
                if (isCrouching || isRunning || !isGrounded) pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID, 15f);
                else pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID, 10f);
            }
            else if (hit.collider.CompareTag("Poi"))
            {
                if (hit.transform.gameObject.name == "Poi_Distiller(Clone)")
                {
                    Poi_DistillerController distillerController = hit.collider.GetComponent<Poi_DistillerController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);

                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.name == "Poi_Dryer(Clone)")
                {
                    Poi_DryerController distillerController = hit.collider.GetComponent<Poi_DryerController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.name == "Poi_Filter(Clone)")
                {
                    Poi_FilterController distillerController = hit.collider.GetComponent<Poi_FilterController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.name == "Poi_Grinder(Clone)")
                {
                    Poi_GrinderController distillerController = hit.collider.GetComponent<Poi_GrinderController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.name == "Poi_Heater(Clone)")
                {
                    Poi_HeaterController distillerController = hit.collider.GetComponent<Poi_HeaterController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.name == "Poi_Smelter(Clone)")
                {
                    Poi_SmelterController distillerController = hit.collider.GetComponent<Poi_SmelterController>();
                    if (distillerController != null)
                    {
                        distillerController.animator.SetTrigger("isHit");
                        for (int i = 0; i < nodeName.Length; i++)
                        {
                            if (nodeName[i] == distillerController.nodeName)
                            {
                                int mixItemCount = distillerController.mixItme;

                                pv.RPC("UpdateMixItem", RpcTarget.AllBuffered, i, mixItemCount);
                            }
                        }
                    }
                }
            }
        }





    }

    private void Attack()
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
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 1;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Axe == 2)
        {
            weapons[0] = weaponsSwitching[3];
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 1;
                GameValue.toolSwitching = false;
            }

        }
        if (GameValue.Pickaxe == 1)
        {
            weapons[1] = weaponsSwitching[1];
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 2;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Pickaxe == 2)
        {
            weapons[1] = weaponsSwitching[4];
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 2;
                GameValue.toolSwitching = false;
            }

        }
        if (GameValue.Shovel == 1)
        {
            weapons[2] = weaponsSwitching[2];
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 0;
                GameValue.toolSwitching = false;
            }

        }
        else if (GameValue.Shovel == 2)
        {
            weapons[2] = weaponsSwitching[5];
            if (GameValue.toolSwitching)
            {
                selectedWeaponIndex = 0;
                GameValue.toolSwitching = false;
            }
        }

        // 무기 번호 키로 무기 교체
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

        // 무기 교체가 필요하면 새로운 무기를 장착

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
        if (index >= 0 && index < nodeItiems.Length)
        {
            nodeItiems[index] = newCount;
            OnInventoryChanged?.Invoke(); // 아이템 변경 시 이벤트 발생
        }
    }

    //todo 멀티테스트
    private void IncreaseLocalPlayerItems()
    {
        // 로컬 플레이어의 아이템 갯수만 증가시킵니다.

        for (int i = 0; i < nodeItiems.Length; i++)
        {
            nodeItiems[i] += 10;
            Debug.Log("아이템 증가" + nodeItiems[i]);
        }
        // 아이템 변경을 UI에 알립니다.
        OnInventoryChanged?.Invoke();
    }


    private void EquipWeapon(int index)
    {
        // 기존에 장착된 무기 비활성화
        foreach (Transform child in weaponHoldPoint)
        {
            Destroy(child.gameObject);
        }

        // 새로운 무기 인스턴스 생성 및 장착
        GameObject weaponInstance = Instantiate(weapons[index], weaponHoldPoint.position, weaponHoldPoint.rotation);
        weaponInstance.transform.SetParent(weaponHoldPoint);
    }
}