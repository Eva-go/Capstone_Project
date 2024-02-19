using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    //���콺 ����
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
        //���� ī�޶� ȸ��
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        //���콺 ī�޶� ���� ������ (+=,-=)
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    private void CharacterRotation()
    {
        //�¿� ĳ���� ȸ��
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

                //	��ũ�� ȭ�� �߾ӿ��� �������� ���� ĳ��Ʈ�� ���� �浹�� ��ü�� �ִٸ�
                if (Physics.Raycast(ray, out hit, Mathf.Infinity) == true)
                {
                    //	�� ��ü�� tag�� BLOCK (Ÿ��)���
                    if (hit.collider.gameObject.tag == "BLOCK")
                    {
                        Vector3 SurfaceVec = hit.normal;        //	�浹�� ť���� ǥ��(���)�� ���� ���͸� ���

                        Vector3 hitCubePos = hit.transform.position;    //	�浹�� ť���� ��ġ�� ���

                        Vector3 InstantiatedCubePos = SurfaceVec + hitCubePos;  //	������ ť���� ��ġ�� ���� ���Ϳ� �浹�� ť���� ��ġ�� ���Ѵ�.

                        //	�׸��� ť�긦 �����Ѵ�.
                        GameObject Obj = Instantiate(Resources.Load("Cube"),
                            InstantiatedCubePos, Quaternion.identity) as GameObject;

                        Obj.gameObject.tag = "BLOCK";
                    }
                }
            }
        }
        
    }
}
