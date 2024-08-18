using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt: MonoBehaviour
{
    [SerializeField]
    private float speed;          // ��Ʈ�� �ӵ�

    [SerializeField]
    private float conveyorSpeed; // �ؽ�ó�� �̵� �ӵ�

    [SerializeField]
    private Vector3 direction;   // ��ü�� �̵��� ����

    [SerializeField]
    private List<GameObject> onBelt; // ��Ʈ ���� ��ü��

    void Start()
    {
        // ���� ��ü�� ȸ������ Quaternion���� �����ɴϴ�.
        Quaternion rotation = gameObject.transform.rotation;
        
        // Quaternion�� ���Ϸ� ������ ��ȯ�մϴ�.
        Vector3 eulerAngles = rotation.eulerAngles;
        
        // ���Ϸ� ������ 360�� ������ ��ȯ�մϴ�.
        eulerAngles = NormalizeEulerAngles(eulerAngles);
        
        // ���Ϸ� ������ ���� direction�� �����մϴ�.
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
            direction = Vector3.zero; // �⺻��
        }
        
    }

    private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
    {
        eulerAngles.x = NormalizeAngle(eulerAngles.x);
        eulerAngles.y = NormalizeAngle(eulerAngles.y);
        eulerAngles.z = NormalizeAngle(eulerAngles.z);
        return eulerAngles;
    }

    // ���� ������ 0-360�� ������ ����ȭ�մϴ�.
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    void Update()
    {
        // �ؽ�ó�� �̵� ó�� (�ʿ��� ��쿡�� ���)
        // material.mainTextureOffset += new Vector2(0, 1) * conveyorSpeed * Time.deltaTime;
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
                // ��Ʈ ���� �� ��ü�� ���� �߰�
                onBelt[i].GetComponent<Rigidbody>()?.AddForce(speed * direction);
                Debug.Log("dir"+ direction);
            }
        }
    }

    // ��ü�� ��Ʈ�� �浹���� ��
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� null�� �ƴϰ� ����Ʈ�� ���ԵǾ� ���� ���� ��� �߰�
        if (collision.gameObject != null && !onBelt.Contains(collision.gameObject))
        {
            onBelt.Add(collision.gameObject);
        }
    }

    // ��ü�� ��Ʈ���� �浹�� ������ ��
    private void OnCollisionExit(Collision collision)
    {
        // �浹�� ���� ��ü�� null�� �ƴϸ� ����Ʈ���� ����
        if (collision.gameObject != null)
        {
            onBelt.Remove(collision.gameObject);
        }
    }
}