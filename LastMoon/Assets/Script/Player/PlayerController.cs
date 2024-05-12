
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class PlayerController : MonoBehaviour
{
    public  PhotonView pv;
    GameObject cam;
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
    void Start()
    {
        pv = GetComponent<PhotonView>();
        myRigid = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera");
        cam.SetActive(false);
    }




    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine)
        {
            cam.SetActive(true);
            Move();
            CameraRotation();
            CharacterRotation();
            Inside();
        }
    }
    private void Inside()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo,5) && hitInfo.collider.tag==("APT"))
        {
            // Get the collider bounds
            Collider collider = hitInfo.collider;
            Bounds bounds = collider.bounds;

            // Calculate the top position of the collider
            Vector3 topPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

            // 이전에 이동한 위치와 비교하여 같으면 이전 위치로 이동
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

        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }




    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
       
    }
}
