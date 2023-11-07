using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

//cube ��ġ

public class PlayerController : MonoBehaviour
{
    
    //�÷��̾� �ӵ� ����
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float crouchSpeed;


    //���� ���� ����
    [SerializeField]
    private float jumpForce;

    //���� ����
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = false;

   
    
    //�þ� �ΰ���
    [SerializeField]
    private float lookSensitivity = 2;

    //ī�޶� �ּ�,�ִ밢
    [SerializeField]
    private float cameraRotationLimit = 45;
    private float currentCameraRotationX;


    //�� ���� ����
    private CapsuleCollider capsuleCollider;

    //���� ���� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //�ʿ� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;


    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        //�ʱ�ȭ
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
        
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
       
    }
    private void FixedUpdate()
    {
        RayCast();
    }
   



    private void RayCast()
    {
       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit))
            {
                //Raycast�� �ǹ��� ������
                if (hit.transform.gameObject.name == "APT(Clone)")
                {
                    SceneManager.LoadScene("Apt1Scene");

                }
                else if (hit.transform.gameObject.name == "Plane")
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
           
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) == true)
            {
                if (hit.collider.gameObject.tag == "Block")
                {
                    Vector3 SurfaceVec = hit.normal;
                    Vector3 hitCubePos = hit.transform.position;
                    Vector3 InstantiatedCubePos = SurfaceVec + hitCubePos;
                    GameObject Obj = Instantiate(Resources.Load("Prefab/Cube_Preview"),InstantiatedCubePos, Quaternion.identity) as GameObject;
                    Obj.gameObject.tag = "Block";
                }
            }
        }
    }
    //�ɱ� �õ�
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //�ɱ� ����
    private void Crouch()
    {
        isCrouch= !isCrouch;

        if(isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    //�ε巯�� �ɱ� ���� ����
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;
       
        while(_posY != applyCrouchPosY)
        {
            count++;
            //������ �̿��� ī�޶� ��ġ�� �̵�
            _posY = Mathf.Lerp(_posY,applyCrouchPosY,0.3f);
            theCamera.transform.localPosition = new Vector3(0f, _posY, 0f);
            if (count > 15) //������ Ƚ���� ����
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    //���� üũ
    private void IsGround()
    {
        //���� Ȯ���� ���� Raycast
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+0.1f);
    }

    //�����õ�
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround) 
        {
            Jump();
        }
    }

    //����
    private void Jump()
    {
        //���� ���¿��� ������ �ɱ� ����
        if (isCrouch)
            Crouch();

        myRigid.velocity = transform.up * jumpForce;
    }

    //�޸��� �õ�
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    //�޸��� ����
    private void Running()
    {
        //���� ���¿��� �޸���� �ɱ� ����
        if (isCrouch)
            Crouch();
        isRun = true;
        applySpeed = runSpeed;
    }

    //�޸��� ���
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    //������
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _MoveHorizontal = transform.right * _moveDirX;
        Vector3 _MoveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_MoveHorizontal + _MoveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity*Time.deltaTime);
    }

    //�¿� ĳ���� ȸ��
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation*Quaternion.Euler(_characterRotationY));
    }

    //���� ī�޶� ȸ��
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-cameraRotationLimit,cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX,0f ,0f);
    
    }

  
}