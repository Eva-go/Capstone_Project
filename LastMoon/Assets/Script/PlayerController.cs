using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    //마우스 감도
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private Camera playerCamera;
    private Rigidbody playerRigidBody;


    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraRotation();
        CharacterRotation();
        BuildBlock();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;
        playerRigidBody.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
    private void CameraRotation()
    {
        //상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        //마우스 카메라 상하 반전시 (+=,-=)
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        playerRigidBody.MoveRotation(playerRigidBody.rotation * Quaternion.Euler(_characterRotationY));
    }


    private void BuildBlock()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //	스크린 화면 중앙에서 직선으로 래이 캐스트로 쏴서 충돌한 물체가 있다면
                if (Physics.Raycast(ray, out hit, Mathf.Infinity) == true)
                {
                    //	그 물체의 tag가 BLOCK (타일)라면
                    if (hit.collider.gameObject.tag == "BLOCK")
                    {
                        Vector3 SurfaceVec = hit.normal;        //	충돌한 큐브의 표면(평면)의 법선 벡터를 얻고

                        Vector3 hitCubePos = hit.transform.position;    //	충돌한 큐브의 위치를 얻고

                        Vector3 InstantiatedCubePos = SurfaceVec + hitCubePos;  //	생성할 큐브의 위치는 법선 벡터와 충돌한 큐브의 위치로 구한다.

                        //	그리고 큐브를 생성한다.
                        GameObject Obj = Instantiate(Resources.Load("Cube"),
                            InstantiatedCubePos, Quaternion.identity) as GameObject;

                        Obj.gameObject.tag = "BLOCK";
                    }
                }
            }
        }
        
    }
}
