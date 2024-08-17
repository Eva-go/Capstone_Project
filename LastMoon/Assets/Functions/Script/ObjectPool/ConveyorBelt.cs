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
        // 초기화가 필요하다면 여기에 작성
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