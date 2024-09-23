using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltTurn : MonoBehaviour
{
    [SerializeField]
    private float speed;          // ��Ʈ�� �ӵ�

    [SerializeField]
    private float conveyorSpeed; // �ؽ�ó�� �̵� �ӵ�

    [SerializeField]
    private Vector3 direction;   // ��ü�� �̵��� ����

    [SerializeField]
    private List<GameObject> onBelt; // ��Ʈ ���� ��ü��

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
        // �ı��� ��ü�� ����Ʈ���� ����
        for (int i = onBelt.Count - 1; i >= 0; i--)
        {
            if (onBelt[i] == null)
            {
                onBelt.RemoveAt(i);
            }
            else
            {
                // ��ü�� Collider�� boxCollider ���� �浹 üũ
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
            // Collider�� bounds�� ����Ͽ� �浹 ���θ� Ȯ���մϴ�.
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
        // ��ü�� ��Ʈ�� �浹���� ��
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
        // ���Ϸ� ������ 360�� ������ ��ȯ�մϴ�.
        eulerAngles = NormalizeEulerAngles(eulerAngles);
        eulerAngles.y = Mathf.Round(eulerAngles.y);
        Debug.Log("�����̼�" + eulerAngles.y);
        // ���Ϸ� ������ ���� direction�� �����մϴ�.
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
        // ��Ʈ�� ������ ������Ʈ�ϰ�, ���Ϸ� ������ �����մϴ�.
        Quaternion rotation = gameObject.transform.rotation;
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles.y -= 90;

        // ��Ʈ�� direction�� �ٽ� �����մϴ�.
        UpdateBeltDirection(eulerAngles);
    }
}