using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public PhotonView pv;
    public static float Hp = 100;
    private GameObject cam;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    [SerializeField] public Camera theCamera;
    [SerializeField] private Camera toolCamera;
    private Rigidbody myRigid;

    //Jump and Crouch
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float crouchHeight = 0.5f;
    private bool isGrounded;
    private bool isCrouching;

    //ray
    private RaycastHit hitInfo;
    private Vector3 previousPosition;
    private bool ins = true;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator Localanimator; //�����÷��̾� ���� �ִϸ��̼�

    public GameObject[] weapons; // ���� ������Ʈ �迭
    public Transform weaponHoldPoint; // ���⸦ ������ �� ��ġ
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
        myRigid = this.GetComponent<Rigidbody>();
        cam = GameObject.Find("Camera");
        cam.SetActive(false);
        // �ʱ� ���� ����
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
            Crouch();
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

            // �÷��̾� ������ ��û
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
        float moveDirX;
        float moveDirZ;
        Vector3 moveHorizontal;
        Vector3 moveVertical;
        Vector3 velocity;
        moveDirX = Input.GetAxisRaw("Horizontal");
        moveDirZ = Input.GetAxisRaw("Vertical");

        moveHorizontal = transform.right * moveDirX;
        moveVertical = transform.forward * moveDirZ;

        velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);

        if(isCrouching)
        {
            if (velocity != Vector3.zero)
            {
                animator.SetBool("isCrouchWalk", true);
            }
            else
            {
                animator.SetBool("isCrouchWalk", false);
            }
           
        }
        else
        {
            animator.SetBool("isCrouchWalk", false);
            if (velocity != Vector3.zero)
            {
                animator.SetBool("isMove", true);
            }
            else
            {
                animator.SetBool("isMove", false);
            }
        }
        
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                //theCamera.transform.position = new Vector3(0, theCamera.transform.transform.position.y - 1, 0);
                //toolCamera.transform.position = new Vector3(0, toolCamera.transform.transform.position.y - 1, 0);
                walkSpeed /= 2;
            }
            else
            {
                //theCamera.transform.position = new Vector3(0, theCamera.transform.transform.position.y + 1, 0);
                //toolCamera.transform.position = new Vector3(0, toolCamera.transform.transform.position.y + 1, 0);
                walkSpeed *= 2;
            }
            animator.SetBool("isCrouch", isCrouching);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
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

        // ���� ��ȣ Ű�� ���� ��ü
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

        // ���� ��ü�� �ʿ��ϸ� ���ο� ���⸦ ����
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
        // ������ ������ ���� ��Ȱ��ȭ
        foreach (Transform child in weaponHoldPoint)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ���� �ν��Ͻ� ���� �� ����
        GameObject weaponInstance = Instantiate(weapons[index], weaponHoldPoint.position, weaponHoldPoint.rotation);
        weaponInstance.transform.SetParent(weaponHoldPoint);
    }
}