using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt: MonoBehaviour
{
    [SerializeField]
    private float speed;          // 벨트의 속도

    [SerializeField]
    private float conveyorSpeed; // 텍스처의 이동 속도

    [SerializeField]
    private Vector3 direction;   // 물체가 이동할 방향

    [SerializeField]
    private List<GameObject> onBelt; // 벨트 위의 물체들

    void Start()
    {
        // 현재 물체의 회전값을 Quaternion으로 가져옵니다.
        Quaternion rotation = gameObject.transform.rotation;
        
        // Quaternion을 오일러 각도로 변환합니다.
        Vector3 eulerAngles = rotation.eulerAngles;
        
        // 오일러 각도를 360도 범위로 변환합니다.
        eulerAngles = NormalizeEulerAngles(eulerAngles);
        
        // 오일러 각도에 따라 direction을 설정합니다.
        if (eulerAngles.y == 0)
        {
            direction = new Vector3(1, 0, 0);
        }
        else if (eulerAngles.y == 180)
        {
            direction = new Vector3(-1, 0, 0);
        }
        else if (eulerAngles.y == 90)
        {
            direction = new Vector3(0, 0, -1);
        }
        else if (eulerAngles.y == 270)
        {
            direction = new Vector3(0, 0, 1);
        }
        else
        {
            Debug.Log("Start dir" + eulerAngles.x + eulerAngles.y);
            direction = Vector3.zero; // 기본값
        }
        
    }

    private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
    {
        eulerAngles.x = NormalizeAngle(eulerAngles.x);
        eulerAngles.y = NormalizeAngle(eulerAngles.y);
        eulerAngles.z = NormalizeAngle(eulerAngles.z);
        return eulerAngles;
    }

    // 개별 각도를 0-360도 범위로 정규화합니다.
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    void Update()
    {
        // 텍스처의 이동 처리 (필요한 경우에만 사용)
        // material.mainTextureOffset += new Vector2(0, 1) * conveyorSpeed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        // 파괴된 객체를 리스트에서 제거
        for (int i = onBelt.Count - 1; i >= 0; i--)
        {
            if (onBelt[i] == null)
            {
                onBelt.RemoveAt(i);
            }
            else
            {
                // 벨트 위의 각 물체에 힘을 추가
                onBelt[i].GetComponent<Rigidbody>()?.AddForce(speed * direction);
                Debug.Log("dir"+ direction);
            }
        }
    }

    // 물체가 벨트와 충돌했을 때
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체가 null이 아니고 리스트에 포함되어 있지 않은 경우 추가
        if (collision.gameObject != null && !onBelt.Contains(collision.gameObject))
        {
            onBelt.Add(collision.gameObject);
        }
    }

    // 물체가 벨트와의 충돌을 끝냈을 때
    private void OnCollisionExit(Collision collision)
    {
        // 충돌이 끝난 객체가 null이 아니면 리스트에서 제거
        if (collision.gameObject != null)
        {
            onBelt.Remove(collision.gameObject);
        }
    }
}