using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltTurn : MonoBehaviour
{
    [SerializeField]
    private float speed;          // 벨트의 속도

    [SerializeField]
    private float conveyorSpeed; // 텍스처의 이동 속도

    [SerializeField]
    private Vector3 direction;   // 물체가 이동할 방향

    [SerializeField]
    private List<GameObject> onBelt; // 벨트 위의 물체들

    [SerializeField]
    private BoxCollider boxCollider;

    private void Start()
    {
        UpdateBeltDirection(gameObject.transform.parent.transform.rotation.eulerAngles);
    }

    private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
    {
        eulerAngles.x = NormalizeAngle(eulerAngles.x);
        eulerAngles.y = NormalizeAngle(eulerAngles.y);
        eulerAngles.z = NormalizeAngle(eulerAngles.z);
        return eulerAngles;
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0f) angle += 360f;
        return angle;
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
                // 물체의 Collider와 boxCollider 간의 충돌 체크
                if (IsCollidingWithBoxCollider(onBelt[i]))
                {
                    UpdateDirectionForObject(onBelt[i]);
                    onBelt[i].GetComponent<Rigidbody>().velocity = direction;
                }
                else
                {
                    onBelt[i].GetComponent<Rigidbody>().velocity = direction;
                }

            }
        }
    }

    private bool IsCollidingWithBoxCollider(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (boxCollider != null)
        {
            // Collider의 bounds를 사용하여 충돌 여부를 확인합니다.
            return boxCollider.bounds.Intersects(objCollider.bounds);
        }
        if (objCollider == null || boxCollider == null)
        {
            return false;
        }
        return false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
        collision.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        // 물체가 벨트와 충돌했을 때
        if (collision.gameObject != null && !onBelt.Contains(collision.gameObject))
        {
            onBelt.Add(collision.gameObject);
        }
        else
        {
            if (collision.gameObject != null)
            {
                onBelt.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
        collision.gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (boxCollider != null && !boxCollider.bounds.Intersects(collision.bounds))
        {
            collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
            collision.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
        else if (boxCollider == null)
        {
            collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
            collision.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
        onBelt.Remove(collision.gameObject);
    }

    private void UpdateBeltDirection(Vector3 eulerAngles)
    {
        // 오일러 각도를 360도 범위로 변환합니다.
        eulerAngles = NormalizeEulerAngles(eulerAngles);
        eulerAngles.y = Mathf.Round(eulerAngles.y);
        Debug.Log("로테이션" + eulerAngles.y);
        // 오일러 각도에 따라 direction을 설정합니다.
        if (eulerAngles.y == 0f || eulerAngles.y == 360f)
        {
            direction = new Vector3(-1, 0, 0);
        }
        else if (eulerAngles.y == 180f)
        {
            direction = new Vector3(1, 0, 0);
        }
        else if (eulerAngles.y == 90f)
        {
            direction = new Vector3(0, 0, 1);
        }
        else if (eulerAngles.y == 270f)
        {
            direction = new Vector3(0, 0, -1);
        }

    }

    private void UpdateDirectionForObject(GameObject obj)
    {
        // 벨트의 방향을 업데이트하고, 오일러 각도를 수정합니다.
        Quaternion rotation = gameObject.transform.rotation;
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles.y -= 90;

        // 벨트의 direction을 다시 설정합니다.
        UpdateBeltDirection(eulerAngles);
    }
}