using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Photon.Pun;
using Photon.Realtime;

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

    public GameObject[] weapons; // ���� ������Ʈ �迭
    public Transform weaponHoldPoint; // ���⸦ ������ �� ��ġ
    private int selectedWeaponIndex = 0;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        myRigid = GetComponent<Rigidbody>();
        cam = GameObject.Find("Camera");
        cam.SetActive(false);
        // �ʱ� ���� ����
        EquipWeapon(selectedWeaponIndex);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            cam.SetActive(true);
            Move();
            CameraRotation();
            CharacterRotation();
            Inside();
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
        }

    }



    private void Switching()
    {
        int previousSelectedWeaponIndex = selectedWeaponIndex;

        // ���콺 �ٷ� ���� ��ü
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex + 1) % weapons.Length;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex - 1 + weapons.Length) % weapons.Length;
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

    private void Inside()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 5) && hitInfo.collider.CompareTag("APT"))
        {
            // Get the collider bounds
            Collider collider = hitInfo.collider;
            Bounds bounds = collider.bounds;

            // Calculate the top position of the collider
            Vector3 topPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

            // ������ �̵��� ��ġ�� ���Ͽ� ������ ���� ��ġ�� �̵�
            if (Input.GetMouseButtonDown(1))
            {
                if (ins)
                {
                    previousPosition = transform.position;
                    transform.position = topPosition;
                    ins = false;
                }
                else
                {
                    transform.position = previousPosition;
                    ins = true;
                }
            }
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

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0)) //(Input.GetButton("Fire1")) ������������ �ݺ�
        {
                animator.SetTrigger("Swing");
                Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f))
            {
                //NodeHp node = hit.collider.GetComponent<NodeHp>();
                // Debug.DrawRay(ray.origin, ray.direction, Color.red,5f);
                // if (node != null)
                // {
                //     Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);
                //     node.TakeDamage(10); // ������ �� �޴� ������
                // }
                NodeController nodeController = hit.collider.GetComponent<NodeController>();
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
                if(nodeController != null)
                {
                    Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);
                    nodeController.TakeDamage();
                }
            }

        }
    }

    private void CharacterRotation()
    {
        // �¿� ĳ���� ȸ��
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        // ���� ī�޶� ȸ��
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}