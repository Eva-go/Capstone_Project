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
        // �ʱ�ȭ�� �ʿ��ϴٸ� ���⿡ �ۼ�
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