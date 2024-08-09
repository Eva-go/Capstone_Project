using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPooler : MonoBehaviour
{
    [SerializeField] Rigidbody rbody;
    [SerializeField] Renderer render;

    [SerializeField] float upFoce = 1f;
    [SerializeField] float sideForce = 0.1f;


    //�Ű������� ������� Ȱ��ȭ�� �ʱ�ȭ ����
    private void OnEnable()
    {
        float xForce = Random.Range(- sideForce, sideForce);
        float yForce = Random.Range(upFoce *0.5f, upFoce);
        float zForce = Random.Range(upFoce - sideForce, sideForce);
        Vector3 force = new Vector3(xForce, yForce, zForce);
        rbody.velocity = force;
        Invoke(nameof(DectiveDelay), 5);
            
    }

    //�Ű������� ���Ͽ� �ٲ�� �ϴ� ����
    public void Setup(Color color)
    {
        render.material.color = color; 
    }

    void DectiveDelay() => gameObject.SetActive(false);

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
    }
}
