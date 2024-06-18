using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public PhotonView pv;

    private GameObject cam;
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    [SerializeField]
    public Camera theCamera;
    private Rigidbody myRigid;

    //ray
    private RaycastHit hitInfo;

    private Vector3 previousPosition;
    private bool ins = true;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator Localanimator; //로컬플레이어 무기 애니메이션

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

    //public GameObject LocalTool;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        myRigid = GetComponent<Rigidbody>();
        cam = GameObject.Find("Camera");
        cam.SetActive(false); 
        // 초기 무기 장착
        EquipWeapon(selectedWeaponIndex,OtherWeaponIndex);
        Cursor.lockState = CursorLockMode.Locked;
        GameValue.setMoney();
        PreViewCam = false;
        //LocalTool.SetActive(true);
    }

    void Update()
    {
        if (pv.IsMine)
        {
            cam.SetActive(true);
            Move();
            if(!Poi)
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
                walkSpeed = 10;
            }
            if(Input.GetKeyDown(KeyCode.F4))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -5f, gameObject.transform.position.z);
            }
            if(Input.GetKeyDown(KeyCode.F5))
            {
                GameValue.GetMomey(1000);
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
                OtherWeaponIndex = 0;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex,0);
               GameValue.toolSwitching = false;
            }
            
        }
        else if (GameValue.Axe == 2 )
        {
            weapons[0] = weaponsSwitching[3];
            if (GameValue.toolSwitching)
            {
                OtherWeaponIndex = 3;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex, 0);
                GameValue.toolSwitching = false;
            }
            
        }
        if (GameValue.Pickaxe == 1 )
        {
            weapons[1] = weaponsSwitching[1];
            if (GameValue.toolSwitching)
            {
                OtherWeaponIndex = 1;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex, 1);
                GameValue.toolSwitching = false;
            }
            
        }
        else if (GameValue.Pickaxe == 2)
        {
            weapons[1] = weaponsSwitching[4];
            if (GameValue.toolSwitching)
            {
                OtherWeaponIndex = 4;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex, 1);
                GameValue.toolSwitching = false;
            }
           
        }
        if (GameValue.Shovel == 1)
        {
            weapons[2] = weaponsSwitching[2];
            if (GameValue.toolSwitching)
            {
                OtherWeaponIndex = 2;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex, 2);
                GameValue.toolSwitching = false;
            }
           
        }
        else if (GameValue.Shovel == 2)
        {
            weapons[2] = weaponsSwitching[5];
            if (GameValue.toolSwitching)
            {
                OtherWeaponIndex = 5;
                pv.RPC("RPC_IndexWeapon", RpcTarget.OthersBuffered, OtherWeaponIndex, 2);
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
            pv.RPC("RPC_EquipWeapon", RpcTarget.OthersBuffered, selectedWeaponIndex,OtherWeaponIndex);
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
    private void RPC_IndexWeapon(int index,int weapon)
    {
         weapons[weapon] = weaponsSwitching[index];
    }

    [PunRPC]
    private void RPC_EquipWeapon(int index, int Otherindex)
    {
        EquipWeapon(index,Otherindex);
    }

    private void EquipWeapon(int index,int Otherindex)
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

    private void Interaction()
    {
        //아파트 레이케스트
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKey(KeyCode.E)&& Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.tag == "APT")
        {
            insideActive = true;
        }
        else
            insideActive = false;

        //포이 레이케스트
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
        if (Input.GetMouseButtonDown(0)&&!Poi) //(Input.GetButton("Fire1")) 누르고있을때 반복
        {
            animator.SetTrigger("Swing");
            Localanimator.SetTrigger("Swing");
        }
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

    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}