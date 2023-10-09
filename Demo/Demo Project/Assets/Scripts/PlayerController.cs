using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

//cube 설치
[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject prefab;
    public GameObject previewPrefab;

}
public class PlayerController : MonoBehaviour
{
   
    static bool Scenes=false;
    bool Q_Cancle = false;
    int Q_count = 0;
    
    //플레이어 속도 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float crouchSpeed;


    //점프 조정 변수
    [SerializeField]
    private float jumpForce;

    //상태 변수
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = false;

   
    
    //시야 민감도
    [SerializeField]
    private float lookSensitivity = 2;

    //카메라 최소,최대각
    [SerializeField]
    private float cameraRotationLimit = 45;
    private float currentCameraRotationX;


    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    //앉은 높이 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;


    //오브젝트 변수
    [SerializeField]
    private GameObject objectSpawn;
    [SerializeField]
    private Craft[] craftCube;
    [SerializeField]
    private Transform tfPlayer;
    private GameObject preview; //미리보기 프리팹

    //물건설치 raycast 변수 선언
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;
    private RaycastHit hitInfo;


    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        //초기화
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
    private void prefabCancel()
    {
        Destroy(preview); 
        preview = null;
    }

    public void SlotClick(int slot)
    {
        preview = Instantiate(craftCube[Q_count].previewPrefab, tfPlayer.position + tfPlayer.forward, Quaternion.identity);
    }

    private void Ray()
    {
        if (Physics.Raycast(tfPlayer.position, tfPlayer.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                preview.transform.position = _location;
            }

        }
    }

    //건물 클릭시 옥상도착
    private void RayCast()
    {
       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E");
                //Raycast가 건물에 맞으면
                if (hit.transform.gameObject.name == "APT(Clone)")
                {
                    SceneManager.LoadScene("Apt1Scene");

                }
                else if (hit.transform.gameObject.name == "Plane")
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray();
                Q_Cancle = false;
                preview = Instantiate(craftCube[Q_count].previewPrefab, tfPlayer.position + tfPlayer.forward, Quaternion.identity);
                if (Q_Cancle != true)
                {
                    Q_count += 1;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            { 
                prefabCancel();
            }
            
        }
    }

    //플레이어가 스폰
    private void SpawnObject(Vector3 spawnPosition)
    {
        Instantiate(objectSpawn,spawnPosition,Quaternion.identity);
    }


    //앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //앉기 동작
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

    //부드러운 앉기 동작 실행
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;
       
        while(_posY != applyCrouchPosY)
        {
            count++;
            //보간을 이용해 카메라 위치를 이동
            _posY = Mathf.Lerp(_posY,applyCrouchPosY,0.3f);
            theCamera.transform.localPosition = new Vector3(0f, _posY, 0f);
            if (count > 15) //보간의 횟수를 조정
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    //지면 체크
    private void IsGround()
    {
        //점프 확인을 위한 Raycast
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+0.1f);
    }

    //점프시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround) 
        {
            Jump();
        }
    }

    //점프
    private void Jump()
    {
        //앉은 상태에서 점프시 앉기 해제
        if (isCrouch)
            Crouch();

        myRigid.velocity = transform.up * jumpForce;
    }

    //달리기 시도
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

    //달리기 실행
    private void Running()
    {
        //앉은 상태에서 달리기시 앉기 해제
        if (isCrouch)
            Crouch();
        isRun = true;
        applySpeed = runSpeed;
    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    //움직임
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _MoveHorizontal = transform.right * _moveDirX;
        Vector3 _MoveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_MoveHorizontal + _MoveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity*Time.deltaTime);
    }

    //좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation*Quaternion.Euler(_characterRotationY));
    }

    //상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-cameraRotationLimit,cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX,0f ,0f);
    
    }

  
}
