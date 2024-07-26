using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    public PhotonView pv;
    public static float Hp = 100f;
    private GameObject cam;
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

    public AudioSource sfx_PlayerHit, sfx_PlayerJump, sfx_PlayerWalk, sfx_PlayerSwing;

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
    public static bool Poi;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    //파도 y축 위치
    private Wavetransform wavetransform;
    private float damageInterval = 1.0f; // interval between damage ticks
    private float tickDamage = 5.0f;     // damage per tick
    private float lastDamageTime = 0;    // time of the last tick damage


    //인벤토리 아이템 갯수
    //public int[] nodeItiems;
    //public int[] mixItiems;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        cam = GameObject.Find("Camera");
        cam.SetActive(false);

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
        //Items();
        //파도 찾기
        wavetransform = FindObjectOfType<Wavetransform>();
        live = true;
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

            if (!Poi)
            {
                CameraRotation();
                CharacterRotation();
            }
            Jump();
            Interaction();
            Attack();
            Switching();
            WaveTic();
            if (Input.GetKey("escape"))
                Application.Quit();
            if (Input.GetKeyDown(KeyCode.F2))
            {
                walkSpeed = 100;
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                walkSpeed = 2.5f;
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
            //if(Input.GetKeyDown(KeyCode.F8))
            //{
            //    for(int i=0;i<6;i++)
            //    {
            //        nodeItiems[i] = 10;
            //        mixItiems[i] = 10;
            //    }
            //}

            // Check if the player is dead
            if (Hp <= 0)
            {
                Die();
            }
        }
    }


    //public void Items()
    //{
    //    for(int i=0;i<6;i++)
    //    {
    //        nodeItiems[i] = 0;
    //        mixItiems[i] = 0;
    //    }
    //}
    public void WaveTic()
    {
        float waveHeight, WaterDepth;
        waveHeight = wavetransform.GetWaveHeight(transform.position.x * 0.1f, transform.position.z * 0.1f, wavetransform.currentTime);
        WaterDepth = waveHeight + wavetransform.waveY - transform.position.y;

        if (WaterDepth > 0)
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, pv.ViewID, tickDamage * wavetransform.currentTime + WaterDepth * WaterDepth / 10);
                lastDamageTime = Time.time;
            }
        }
    }
    private void Die()
    {
        if (pv.IsMine)
        {
            Poi = false;
            Debug.Log("Player died, starting respawn process.");
            if(live)
            {
                PhotonNetwork.Destroy(gameObject);
                live = false;
                reSpwan();
            }
           
        }
    }
    
    private void reSpwan()
    {
        if(pv.IsMine&&!live)
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
        //sfx_PlayerWalk.enabled = false;

        if (!isRunning)
        {
            animator.SetBool("isRuns", false);
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            if (moveDirX != 0 || moveDirZ != 0)
            {
                if (isGrounded) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;
                animator.SetBool("isMove", true);
                float moveDirY = myRigid.velocity.y;

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded)
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
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
        }
    }

    private void Run()
    {
        if (!isCrouching)
            isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isRunning && !isCrouching)
        {
            animator.SetBool("isRuns", true);
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");
            if (moveDirX != 0 || moveDirZ != 0)
            {
                if (isGrounded) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;
                float moveDirY = myRigid.velocity.y;

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded)
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
                if (isGrounded) sfx_PlayerWalk.mute = false;
                else sfx_PlayerWalk.mute = true;
                animator.SetBool("isCrouchWalk", true);
                float moveDirY = myRigid.velocity.y;

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;
                if (isGrounded)
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
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "PLANE")
        {
            isGrounded = false;
            myRigid.drag = 1;
            animator.SetBool("isGrounded", false);
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
            Poi=!Poi;
        }
    }

    public void Attack_Time()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f) && hit.collider.CompareTag("Node"))
        {
            NodeController nodeController = hit.collider.GetComponent<NodeController>();
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
            if (nodeController != null)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);
                if (nodeController.Node_Type == selectedWeaponIndex)
                {
                    nodeController.TakeDamage(10f * selectedWeaponStrength, true);
                }
                else
                {
                    nodeController.TakeDamage(5f * selectedWeaponStrength, false);

                }
            }
        }
        else if (Physics.Raycast(ray, out hit, 5f) && hit.collider.CompareTag("Player"))
        {
            // 플레이어 공격 처리
            pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID, 10);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButton(0) && !Poi)
        {
            if (!sfx_PlayerSwing.isPlaying)
            {
                sfx_PlayerSwing.Play();
                if (isCrouching)
                {
                    animator.SetTrigger("Crouch_Swing");
                    Localanimator.SetTrigger("Crouch_Swing");
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