using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public PhotonView pv;
    public static float Hp = 100;
    private GameObject cam;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    [SerializeField] public Camera theCamera;
    private Rigidbody myRigid;

    //ray
    private RaycastHit hitInfo;
    private Vector3 previousPosition;
    private bool ins = true;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator Localanimator; //로컬플레이어 무기 애니메이션

    public GameObject[] weapons; // 무기 오브젝트 배열
    public Transform weaponHoldPoint; // 무기를 장착할 손 위치
    public GameObject[] weaponsSwitching;
    private int selectedWeaponIndex = 0;
    private int OtherWeaponIndex = 0;

    public static int getMoney;
    public GameObject insidegameObject;
    public static bool insideActive;
    public static bool PreViewCam;
    public static bool Poi;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        myRigid = GetComponent<Rigidbody>();
        cam = GameObject.Find("Camera");
        cam.SetActive(false);
        // 초기 무기 장착
        EquipWeapon(selectedWeaponIndex);
        Cursor.lockState = CursorLockMode.Locked;
        
        PreViewCam = false;
        GameValue.setMoney();
    }

    void Update()
    {
        if (pv.IsMine)
        {
            cam.SetActive(true);
            Move();
            if (!Poi)
            {
                CameraRotation();
                CharacterRotation();
            }
            Interaction();
            Attack();
            Switching();

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
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -5f, gameObject.transform.position.z);
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

            // Check if the player is dead
            if (Hp <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (pv.IsMine)
        {
            Debug.Log("Player died, starting respawn process.");
            PhotonNetwork.Destroy(gameObject);

            // 플레이어 리스폰 요청
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
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);

        if (velocity != Vector3.zero)
        {
            animator.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
        }
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
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
            Poi = true;
        }
    }

    public void Attack_Time()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            NodeController nodeController = hit.collider.GetComponent<NodeController>();
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
            if (nodeController != null)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);
                nodeController.TakeDamage(10);
            }
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !Poi)
        {
            animator.SetTrigger("Swing");
            Localanimator.SetTrigger("Swing");
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