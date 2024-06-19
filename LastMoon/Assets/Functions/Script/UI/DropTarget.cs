using UnityEngine;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour
{
    public Sprite newSprite; // ������ �̹���

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            GetComponent<Image>().sprite = newSprite; // �̹��� ����
            collision.gameObject.GetComponent<DraggableObject>().ResetPosition(); // ���� ��ġ�� ���ư���
        }
    }
}